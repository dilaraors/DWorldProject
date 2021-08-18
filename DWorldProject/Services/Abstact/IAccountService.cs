using DWorldProject.Data.Entities.Account;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IAccountService
    {
        ServiceResult<UserResponseModel> Register(UserRequestModel model);
        ServiceResult<LoginModel> Login(UserRequestModel model);
        //Task<ServiceResult<bool>> Logout();
    }
}
