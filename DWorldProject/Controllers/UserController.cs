using System.Threading.Tasks;
using DWorldProject.ErrorHandler;
using DWorldProject.Migrations;
using DWorldProject.Models.Request;
using DWorldProject.Services;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    [AllowAnonymous]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController(IUserService userService, IConfiguration configuration) : base(configuration)
        {
            _userService = userService;
        }


        [HttpGet("[action]")]
        public IActionResult Get()
        {
            var userId = GetUserIdFromContext();
            var user = _userService.GetById(userId);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "No User is found!"));
            }

            return Ok(new ApiOkResponse(user.Data));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = GetUserIdFromContext();
            var result = await _userService.UploadProfileImageToS3(file, userId);

            if (result.ResultType == ServiceResultType.Fail)
            {
                return BadRequest(new ApiResponse(400, "Image could not uploaded!"));
            }

            return Ok(new ApiOkResponse(result.Data));
        }

        [HttpGet("[action]")]
        public IActionResult GetProfileImage()
        {
            var userId = GetUserIdFromContext();
            var result = _userService.GetProfileImage(userId);

            if (result.ResultType == ServiceResultType.Fail)
            {
                return BadRequest(new ApiResponse(400, "Image could not found!"));
            }

            return Ok(new ApiOkResponse(result.Data));
        }

        [HttpPost("[action]")]
        public IActionResult UpdateUserInfo([FromBody]UserRequestModel model)
        {
            var userId = GetUserIdFromContext();
            var result = _userService.UpdateUserInfo(model, userId);

            if (result.ResultType == ServiceResultType.Fail)
            {
                return BadRequest(new ApiResponse(400, "User could not found!"));
            }

            return Ok(new ApiOkResponse(result.Data));
        }

    }
}
