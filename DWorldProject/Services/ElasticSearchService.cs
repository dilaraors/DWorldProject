using DWorldProject.Models.ViewModel;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWorldProject.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        public readonly IConfiguration _configuration;
        public readonly IElasticClient _client;

        public ElasticSearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = CreateInstance();
        }

        private ElasticClient CreateInstance()
        {
            var connSettings =
                new ConnectionSettings(new Uri(_configuration.GetSection("Elasticsearch").GetSection("URL").Value));

            return new ElasticClient(connSettings);
        }

        public async Task CheckIndex(string indexName)
        {
            var index = await _client.Indices.ExistsAsync(indexName);
            if (index.Exists) return;

            var response = await _client.Indices.CreateAsync(indexName,
                ci => ci.LogMapping()
                    .Settings(s => s.NumberOfShards(3).NumberOfReplicas(1)));
            return;
        }

        public async Task InsertDocument(string indexName, BlogPostLogModel logModel)
        {
            var response = await _client.CreateAsync(logModel, q => q.Index(indexName));
            if (response.ApiCall?.HttpStatusCode == 409)
            {
                await _client.UpdateAsync<BlogPostLogModel>(response.Id, a => a.Index(indexName).Doc(logModel));
            }
        }

        public async Task<BlogPostLogModel> GetDocument(string indexName, int id)
        {
            var response = await _client.GetAsync<BlogPostLogModel>(id, q => q.Index(indexName));
            return response.Source;
        }

        public async Task<List<BlogPostLogModel>> GetDocuments(string indexName, BlogPostLogModel logModel)
        {
            var response = await _client.SearchAsync<BlogPostLogModel>(q => q.Index(indexName).Scroll("5m"));
            return response.Documents.ToList();
        }

    }
}
