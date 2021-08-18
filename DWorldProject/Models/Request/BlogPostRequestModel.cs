using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

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
        public string HeaderImageURL { get; set; }
        public List<IFormFile> ImageGalleryFile { get; set; }
        public List<string> ImageGallery { get; set; }
        public string YouTubeVideoURL { get; set; }
    }

    public class BlogPostSectionRequestModel
    {
        public int SectionId { get; set; }
        public string TopicName { get; set; }
    }
}
