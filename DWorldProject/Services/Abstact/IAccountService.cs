using DWorldProject.Data.Entities.Account;
using DWorldProject.Models.Request;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IAccountService
    {
        Task<ServiceResult<RegisterModel>> Register(RegisterModel model);
        Task<ServiceResult<LoginModel>> Login(UserRequestModel model);
        Task<ServiceResult<bool>> Logout();
    }
}
