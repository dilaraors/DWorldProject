using DWorldProject.Data.Entities.Account;
using DWorldProject.ErrorHandler;
using DWorldProject.Models.Request;
using DWorldProject.Services;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService, IConfiguration configuration) : base(configuration)
        {
            _accountService = accountService;
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody] UserRequestModel model)
        {
            var result = _accountService.Login(model);
            if (result.ResultType == ServiceResultType.Fail)
            {
                return NotFound(result);
            }

            return Ok(new ApiOkResponse(result.Data));
        }
        
        [HttpPost("[action]")]
        public IActionResult Register([FromBody]UserRequestModel model)
        {
            var result = _accountService.Register(model);
            if (result.ResultType == ServiceResultType.Fail)
            {
                return NotFound(result);
            }

            return Ok(new ApiOkResponse(result.Data));
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> Logout()
        //{
        //    var result = await _accountService.Logout();
        //    if (result.ResultType == ServiceResultType.Fail)
        //    {
        //        return NotFound(result);
        //    }

        //    return Ok(new ApiOkResponse(result.Data));
        //}
    }
}
