using System.Threading.Tasks;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using Microsoft.AspNetCore.Http;

namespace DWorldProject.Services.Abstact
{
    public interface IUserService
    {
        ServiceResult<UserResponseModel> GetById(int id);
        ServiceResult<UserResponseModel> GetDWUser(string contextUserId);
        Task<ServiceResult<bool>> UploadProfileImageToS3(IFormFile file, int id);
        ServiceResult<string> GetProfileImage(int id);
        ServiceResult<UserResponseModel> UpdateUserInfo(UserRequestModel model, int userId);
    }
}
