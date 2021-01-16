using DWorldProject.Enums;
using System;

namespace DWorldProject.Models.Response
{
    public class BlogPostResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string HeaderImageUrl { get; set; }
        public string[] ImageGallery { get; set; }
        public string YouTubeVideoURL { get; set; }
        public int? SectionId { get; set; }
        public int? TopicId { get; set; }
        public string TopicName { get; set; }
        public string[] HomePageImages { get; set; }
    }
}
