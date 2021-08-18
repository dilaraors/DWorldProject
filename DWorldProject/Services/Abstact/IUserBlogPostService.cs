using DWorldProject.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using DWorldProject.Models.Request;

namespace DWorldProject.Services.Abstact
{
    public interface IUserBlogPostService
    {
        Task<ServiceResult<List<UserBlogPostResponseModel>>> GetByType(int blogType, int userId);
        ServiceResult<UserBlogPostByTypeResponseModel> AddByType(UserBlogPostByTypeRequestModel request, int userId);
        ServiceResult<UserBlogPostByTypeResponseModel> DeleteByType(UserBlogPostByTypeRequestModel request, int userId);
        ServiceResult<bool> GetBlogPostByTypeExistence(UserBlogPostByTypeRequestModel request, int userId);

    }
}
