using DWorldProject.Models.IyziPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    public class IyziPayController : BaseController
    {
        public ICheckoutFormSample _checkoutFormSample;

        public IyziPayController(IConfiguration configuration, ICheckoutFormSample checkoutFormSample) : base(configuration)
        {
            _checkoutFormSample = checkoutFormSample;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult Initialize()
        {
            var res = _checkoutFormSample.Should_Initialize_Checkout_Form();

            return Ok(res);
        }
        
        [HttpPost("[action]")]
        [AllowAnonymous]
        public void Finalize(CheckoutFormFinalizeModel request)
        {
            var res = _checkoutFormSample.Should_Retrieve_Checkout_Form_Result(request);

          //  return Ok(res);
        }

    }
}
