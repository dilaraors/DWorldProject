using DWorldProject.Data.Entities;
using DWorldProject.Models.ViewModel;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.Enums;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Services
{
    public class ElasticLogService : IElasticLogService
    {
        private readonly IConfiguration _config;
        private readonly IElasticSearchService _elasticSearchService;

        public ElasticLogService(IConfiguration config, IElasticSearchService elasticSearchService)
        {
            _config = config;
            _elasticSearchService = elasticSearchService;
        }

        public void LogChange(OperationType type, BlogPost model)
        {
            var indexName = _config.GetSection("Elasticsearch").GetSection("IndexName").Value;
            var logModel = new BlogPostLogModel()
            {
                Id = model.Id,
                OperationType = (int)type,
                SectionId = model.SectionId,
                Title = model.Title,
                TopicId = model.TopicId,
                UserId = model.UserId
            };
            _elasticSearchService.InsertDocument(indexName, logModel);
        }

        public void CheckIndex()
        {
            var indexName = _config.GetSection("Elasticsearch").GetSection("IndexName").Value;
            _elasticSearchService.CheckIndex(indexName);
        }
    }
}
