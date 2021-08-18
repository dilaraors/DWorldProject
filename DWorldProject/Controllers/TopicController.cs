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

            return Ok(new ApiOkResponse(topic));
        }


        [HttpGet("[action]/{sectionId}")]
        public IActionResult GetBySectionId([FromRoute] int sectionId)
        {
            var topic = _topicService.GetBySectionId(sectionId);

            return Ok(new ApiOkResponse(topic));
        }

        [HttpGet("[action]")]
        public IActionResult Get()
        {
            var topics = _topicService.Get();

            return Ok(new ApiOkResponse(topics));
        }

        [HttpPost("[action]")]
        public IActionResult Add([FromBody]TopicModel model)
        {
            var topic = _topicService.Add(model);

            return Ok(new ApiOkResponse(topic));
        }
        
        [HttpPost("[action]")]
        public IActionResult Edit([FromBody]TopicModel model)
        {
            var topic = _topicService.Edit(model);

            return Ok(new ApiOkResponse(topic));
        }
        
        [HttpPost("[action]/{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            var topic = _topicService.Delete(id);

            return Ok(new ApiOkResponse(topic));
        }

    }
}
