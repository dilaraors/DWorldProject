using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            if (userId != null)
            {
                return Convert.ToInt32(userId);
            }
            return 0;
        }

    }
}
