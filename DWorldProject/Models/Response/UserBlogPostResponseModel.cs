namespace DWorldProject.Models.Response
{
    public class UserBlogPostResponseModel : BlogPostResponseModel
    {
        public int UserId { get; set; }
        public int BlogType { get; set; }
    }
}
