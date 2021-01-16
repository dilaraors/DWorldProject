using DWorldProject.ErrorHandler;
using DWorldProject.Models.Request;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DWorldProject.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("EnableCors")]
    [Authorize]
    [AllowAnonymous]
    public class BlogPostController : BaseController
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService, IConfiguration configuration) : base(configuration)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Get()
        {
            var blogPost = await _blogPostService.Get();

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var blogPost = await  _blogPostService.GetById(id);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        // PUT: api/BlogPosts/Update/5
        [HttpPut("[action]")]
        public IActionResult Update([FromBody] BlogPostRequestModel blogPostModel)
        {
            var blogPost = _blogPostService.Update(blogPostModel);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        // POST: api/BlogPosts/Add
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] BlogPostRequestModel blogPostModel)
        {
            var blogPost = _blogPostService.Add(blogPostModel);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }

        // DELETE: api/BlogPosts/Delete/5
        [HttpDelete("[action]/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var blogPost = _blogPostService.Delete(id);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.data));
        }


    }
}
