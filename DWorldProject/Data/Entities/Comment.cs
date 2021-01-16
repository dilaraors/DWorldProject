using DWorldProject.Models;

namespace DWorldProject.Data.Entities
{
    public class Comment : BaseEntity
    {
        public int BlogPostId { get; set; }
        public string CommentUrl { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
