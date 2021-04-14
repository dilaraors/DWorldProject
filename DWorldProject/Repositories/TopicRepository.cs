using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;

namespace DWorldProject.Repositories
{
    public class TopicRepository : EntityBaseRepository<Topic>, ITopicRepository
    {
        public readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
