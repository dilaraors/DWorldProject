using DWorldProject.Data.Entities;

namespace DWorldProject.Models.Response
{
    public class UserBlogPostByTypeResponseModel
    {
        public int Type { get; set; }
        public int UserId { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
