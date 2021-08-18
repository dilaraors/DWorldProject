using System;
using System.Linq;
using DWorldProject.ErrorHandler;
using DWorldProject.Models.Request;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            return Ok(new ApiOkResponse(blogPost.Data));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var blogPost = await _blogPostService.GetById(id);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetBySection([FromBody]BlogPostSectionRequestModel model)
        {
            var blogPosts = await _blogPostService.GetBySection(model.SectionId, model.TopicName);

            return Ok(new ApiOkResponse(blogPosts.Data));
        }

        // PUT: api/BlogPosts/Update/5
        [HttpPut("[action]")]
        public async Task<IActionResult> Update()
        {
            var files = Request.Form.Files.ToList();
            var keyValuePairs = Request.Form.ToList();
            var blogPostModel = new BlogPostRequestModel();
            var imageList = new List<IFormFile>();
            foreach (var file in files)
            {
                if (file.Name == nameof(blogPostModel.HeaderImageFile))
                {
                    blogPostModel.HeaderImageFile = file;
                }
                else if (file.Name == nameof(blogPostModel.ImageGalleryFile))
                {
                    imageList.Add(file);
                }
            }

            blogPostModel.ImageGalleryFile = imageList;

            foreach (var pair in keyValuePairs)
            {
                if (pair.Key == nameof(blogPostModel.Id))
                {
                    blogPostModel.Id = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == nameof(blogPostModel.Title))
                {
                    blogPostModel.Title = pair.Value;
                }
                else if (pair.Key == nameof(blogPostModel.Body))
                {
                    blogPostModel.Body = pair.Value;
                }
                else if (pair.Key == nameof(blogPostModel.TopicId))
                {
                    blogPostModel.TopicId = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == nameof(blogPostModel.SectionId))
                {
                    blogPostModel.SectionId = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == nameof(blogPostModel.YouTubeVideoURL))
                {
                    blogPostModel.YouTubeVideoURL = pair.Value;
                }
            }

            var blogPost = await _blogPostService.Update(blogPostModel);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
        }

        // POST: api/BlogPosts/Add
        [HttpPost("[action]")]
        public async Task<IActionResult> Add()
        {
            var files = Request.Form.Files.ToList();
            var keyValuePairs = Request.Form.ToList();
            var blogPostModel = new BlogPostRequestModel();
            foreach (var file in files)
            {
                if (file.Name == nameof(blogPostModel.HeaderImageFile))
                {
                    blogPostModel.HeaderImageFile = file;
                }
                else if (file.Name == nameof(blogPostModel.ImageGalleryFile))
                {
                    //blogPostModel.ImageGalleryFile = file;
                }
            }

            foreach (var pair in keyValuePairs)
            {
                if (pair.Key == nameof(blogPostModel.Title))
                {
                    blogPostModel.Title = pair.Value;
                }
                else if (pair.Key == nameof(blogPostModel.Body))
                {
                    blogPostModel.Body = pair.Value;
                }
                else if (pair.Key == nameof(blogPostModel.TopicId))
                {
                    blogPostModel.TopicId = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == nameof(blogPostModel.SectionId))
                {
                    blogPostModel.SectionId = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == nameof(blogPostModel.YouTubeVideoURL))
                {
                    blogPostModel.YouTubeVideoURL = pair.Value;
                }
            }
            var blogPost = await _blogPostService.Add(blogPostModel);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse(404, "No BlogPost is found!"));
            }

            return Ok(new ApiOkResponse(blogPost.Data));
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

            return Ok(new ApiOkResponse(blogPost.Data));
        }


    }
}
