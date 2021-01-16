using DWorldProject.Data.Entities.Account;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IAccountService
    {
        Task<ServiceResult<RegisterModel>> Register(RegisterModel model);
        Task<ServiceResult<LoginModel>> Login(LoginModel model);
        Task<ServiceResult<bool>> Logout();
    }
}
