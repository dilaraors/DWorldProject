using DWorldProject.Enums;

namespace DWorldProject.Models.Request
{
    public class BlogPostRequestModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int SectionId { get; set; }
        public int TopicId { get; set; }
    }
}
