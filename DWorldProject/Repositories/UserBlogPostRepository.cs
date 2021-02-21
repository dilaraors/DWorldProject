using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;

namespace DWorldProject.Repositories
{
    public class UserBlogPostRepository : EntityBaseRepository<UserBlogPost>, IUserBlogPostRepository
    {
        private readonly ApplicationDbContext _context;
        public UserBlogPostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
