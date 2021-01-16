using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;

namespace DWorldProject.Repositories
{
    public class BlogPostRepository : EntityBaseRepository<BlogPost>, IBlogPostRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
