using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;

namespace DWorldProject.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        public readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
