using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;
using System.Threading.Tasks;

namespace DWorldProject.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private UserRepository _userRepository;
        private BlogPostRepository _blogPostRepository;
        private UserBlogPostRepository _userBlogPostRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IUserRepository Users => _userRepository = _userRepository ?? new UserRepository(_context);

        public IBlogPostRepository BlogPosts => _blogPostRepository = _blogPostRepository ?? new BlogPostRepository(_context);
        public IUserBlogPostRepository UserBlogPosts => _userBlogPostRepository = _userBlogPostRepository ?? new UserBlogPostRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
