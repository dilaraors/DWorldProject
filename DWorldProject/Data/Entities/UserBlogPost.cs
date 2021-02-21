namespace DWorldProject.Data.Entities
{
    public class UserBlogPost : BaseEntity
    {
        public int BlogPostId { get; set; }
        public int UserId { get; set; }
        public int BlogType { get; set; }

        public BlogPost BlogPost { get; set; }
        public User User { get; set; }
    }
}
