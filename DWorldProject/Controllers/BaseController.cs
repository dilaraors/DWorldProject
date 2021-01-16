using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Web.Http;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public IConfiguration _configuration;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("[action]")]
        public int GetUserIdFromContext()
        {
            var user = HttpContext.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userId != null)
            {
                return Convert.ToInt32(userId.Value);
            }
            return 0;
        }
    }
}
