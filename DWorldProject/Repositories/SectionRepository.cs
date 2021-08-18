using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;

namespace DWorldProject.Repositories
{
    public class SectionRepository : EntityBaseRepository<Section>, ISectionRepository
    {
        public readonly ApplicationDbContext _context;

        public SectionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
