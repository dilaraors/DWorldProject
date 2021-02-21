using Microsoft.AspNetCore.Http;

namespace DWorldProject.Models.Request
{
    public class UploadProfileImageRequestModel
    {
        public IFormFile file { get; set; }
    }
}
