using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IBlogPostService
    {
        Task<ServiceResult<List<BlogPostResponseModel>>> Get();
        Task<ServiceResult<BlogPostResponseModel>> GetById(int id);
        Task<ServiceResult<BlogPostResponseModel>> Update(BlogPostRequestModel blogPostModel);
        Task<ServiceResult<BlogPostResponseModel>> Add(BlogPostRequestModel blogPostModel);
        ServiceResult<BlogPostResponseModel> Delete(int id);
        Task<ServiceResult<List<BlogPostResponseModel>>> GetBySection(int sectionId, string topicName);
    }
}
