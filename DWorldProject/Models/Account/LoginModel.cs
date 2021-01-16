using System.ComponentModel.DataAnnotations;

namespace DWorldProject.Data.Entities.Account
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
