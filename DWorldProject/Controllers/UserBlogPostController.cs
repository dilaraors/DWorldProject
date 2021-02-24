using System.Linq;
using System.Security.Claims;
using DWorldProject.Data.Entities;
using DWorldProject.ErrorHandler;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using DWorldProject.Models.Request;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    [AllowAnonymous]
    public class UserBlogPostController : BaseController
    {
        private readonly IUserBlogPostService _userBlogPostService;
        private readonly IAccountService _accountService;
        private readonly UserManager<IdentityUser> _userManager;

        public UserBlogPostController(IUserBlogPostService userBlogPostService, IAccountService accountService, UserManager<IdentityUser> userManager, 
            IConfiguration configuration) : base(configuration)
        {
            _userBlogPostService = userBlogPostService;
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpPost("[action]")]
        public IActionResult AddByType([FromBody] UserBlogPostByTypeRequestModel model)
        {

            var blogPost = _userBlogPostService.AddByType(model);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        [HttpGet("[action]/{type}/{userId}")]
        public async Task<IActionResult> GetByType(int type, int userId)
        {

            var blogPost = await _userBlogPostService.GetByType(type, userId);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        [HttpPost("[action]")]
        public IActionResult GetBlogPostByTypeExistence([FromBody] UserBlogPostByTypeRequestModel model)
        {

            var existance = _userBlogPostService.GetBlogPostByTypeExistence(model);

            if (existance == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost Existance is found!"));
            }

            return Ok(new ApiOkResponse(existance.data));
        }

        [HttpPost("[action]")]
        public IActionResult DeleteByType([FromBody] UserBlogPostByTypeRequestModel model)
        {

            var blogPost = _userBlogPostService.DeleteByType(model);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is deleted!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }
    }
}
