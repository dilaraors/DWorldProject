using DWorldProject.ErrorHandler;
using DWorldProject.Models.ViewModel;
using DWorldProject.Services;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    [AllowAnonymous]
    public class SectionController: BaseController
    {
        private readonly ISectionService _sectionService;
        public SectionController(ISectionService sectionService, IConfiguration configuration) : base(configuration)
        {
            _sectionService = sectionService;
        }

        [HttpGet("[action]")]
        public IActionResult Get()
        {
            var sections = _sectionService.Get();
            return Ok(new ApiOkResponse(sections));
        }

        [HttpPost("[action]")]
        public IActionResult Add([FromBody] SectionModel model)
        {
            var section = _sectionService.Add(model);

            return Ok(new ApiOkResponse(section));
        }

        [HttpPost("[action]")]
        public IActionResult Edit([FromBody] SectionModel model)
        {
            var section = _sectionService.Edit(model);

            return Ok(new ApiOkResponse(section));
        }

        [HttpPost("[action]/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var section = _sectionService.Delete(id);

            return Ok(new ApiOkResponse(section));
        }
    }
}
