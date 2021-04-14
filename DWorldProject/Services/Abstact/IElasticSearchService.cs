using DWorldProject.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IElasticSearchService
    {
        Task CheckIndex(string indexName);
        Task InsertDocument(string indexName, BlogPostLogModel logModel);
        Task<BlogPostLogModel> GetDocument(string indexName, int id);
        Task<List<BlogPostLogModel>> GetDocuments(string indexName, BlogPostLogModel logModel);
    }
}
