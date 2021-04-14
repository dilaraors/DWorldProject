using DWorldProject.ErrorHandler;
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
    public class TopicController : BaseController
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService, IConfiguration configuration) : base(configuration)
        {
            _topicService = topicService;
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var topic = _topicService.GetById(id);

            if (topic.ResultType == ServiceResultType.Fail)
            {
                return NotFound(new ApiResponse(404, "No Topic is found!"));
            }

            return Ok(new ApiOkResponse(topic));
        }

        [HttpGet("[action]")]
        public IActionResult Get()
        {
            var topics = _topicService.Get();

            if (topics.ResultType == ServiceResultType.Fail)
            {
                return NotFound(new ApiResponse(404, "No Topic is found!"));
            }

            return Ok(new ApiOkResponse(topics));
        }

        [HttpGet("[action]")]
        public IActionResult GetSections()
        {
            var sections = _topicService.GetSections();

            if (sections.ResultType == ServiceResultType.Fail)
            {
                return NotFound(new ApiResponse(404, "No Section is found!"));
            }

            return Ok(new ApiOkResponse(sections));
        }
    }
}
