using System.Collections.Generic;

namespace DWorldProject.Data.Entities
{
    public class BlogPost : BaseEntity
    {
        public int AWSImageId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int SectionId { get; set; }
        public int TopicId { get; set; }
        public int? UserId { get; set; }
        public string YouTubeVideoURL { get; set; }

        public User User { get; set; }
        public Topic Topic { get; set; }
        public ICollection<Comment> CommentIds { get; set; }
    }
}
