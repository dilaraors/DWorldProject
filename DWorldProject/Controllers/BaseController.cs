using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
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

        public int GetUserIdFromContext()
        {
            var user = HttpContext.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId");

            return userId != null ? Convert.ToInt32(userId.Value) : 0;
        }

        public int GetRoleIdFromContext()
        {
            var user = HttpContext.User;
            var roleId = user.Claims.FirstOrDefault(c => c.Type == "RoleId");

            return roleId != null ? Convert.ToInt32(roleId.Value) : 0;
        }

    }
}
