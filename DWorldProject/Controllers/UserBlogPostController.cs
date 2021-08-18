using DWorldProject.ErrorHandler;
using DWorldProject.Models.Request;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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

        public UserBlogPostController(IUserBlogPostService userBlogPostService, IAccountService accountService,
            IConfiguration configuration) : base(configuration)
        {
            _userBlogPostService = userBlogPostService;
            _accountService = accountService;
        }

        [HttpPost("[action]")]
        public IActionResult AddByType([FromBody] UserBlogPostByTypeRequestModel model)
        {
            var userId = GetUserIdFromContext();
            var blogPost = _userBlogPostService.AddByType(model, userId);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
        }

        [HttpGet("[action]/{type}")]
        public async Task<IActionResult> GetByType(int type)
        {
            var userId = GetUserIdFromContext();
            var blogPost = await _userBlogPostService.GetByType(type, userId);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
        }

        [HttpPost("[action]")]
        public IActionResult GetBlogPostByTypeExistence([FromBody] UserBlogPostByTypeRequestModel model)
        {

            var userId = GetUserIdFromContext();
            var existance = _userBlogPostService.GetBlogPostByTypeExistence(model, userId);

            if (existance == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost Existance is found!"));
            }

            return Ok(new ApiOkResponse(existance.Data));
        }

        [HttpPost("[action]")]
        public IActionResult DeleteByType([FromBody] UserBlogPostByTypeRequestModel model)
        {
            var userId = GetUserIdFromContext();
            var blogPost = _userBlogPostService.DeleteByType(model, userId);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is deleted!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
        }
    }
}
