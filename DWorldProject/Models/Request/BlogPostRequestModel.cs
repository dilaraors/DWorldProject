using System.Collections.Generic;
using DWorldProject.Enums;
using Microsoft.AspNetCore.Http;

namespace DWorldProject.Models.Request
{
    public class BlogPostRequestModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int? TopicId { get; set; }
        public int? SectionId { get; set; }
        public IFormFile HeaderImageFile { get; set; }
        public string HeaderImageUrl { get; set; }
        public List<IFormFile> ImageGalleryFile { get; set; }
        public List<string> ImageGallery { get; set; }
        public string YouTubeVideoURL { get; set; }
    }
}
