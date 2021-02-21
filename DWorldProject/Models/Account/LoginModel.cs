using DWorldProject.Models.Response;

namespace DWorldProject.Data.Entities.Account
{
    public class LoginModel
    {
        public UserResponseModel User { get; set; }
        public string JwtToken { get; set; }
    }
}
