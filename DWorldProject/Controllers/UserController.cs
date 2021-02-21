using System.Threading.Tasks;
using DWorldProject.ErrorHandler;
using DWorldProject.Migrations;
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


        [HttpGet("[action]/{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "No User is found!"));
            }

            return Ok(new ApiOkResponse(user.data));
        }

        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file, int id)
        {
            var result = await _userService.UploadProfileImageToS3(file, id);

            if (result.resultType == ServiceResultType.Fail)
            {
                return BadRequest(new ApiResponse(400, "Image could not uploaded!"));
            }

            return Ok(new ApiOkResponse(result.data));
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetProfileImage(int id)
        {
            var result = _userService.GetProfileImage(id);

            if (result.resultType == ServiceResultType.Fail)
            {
                return BadRequest(new ApiResponse(400, "Image could not found!"));
            }

            return Ok(new ApiOkResponse(result.data));
        }

    }
}
