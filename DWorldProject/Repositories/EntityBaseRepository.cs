using DWorldProject.Data.Entities;
using DWorldProject.Entities;
using DWorldProject.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DWorldProject.Repositories
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
            where T : BaseEntity, new()
    {

        private ApplicationDbContext _context;

        #region Constructor
        public EntityBaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion
        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();

        }
        public virtual IQueryable<T> AsQueryable()
        {
            return _context.Set<T>();
        }

        public virtual async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }
        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsEnumerable();
        }

        public virtual IQueryable<T> AllIncludingAsQueryable(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate,
                                   params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }                                                                                                                                                                               

            return query.Where(predicate).FirstOrDefault();
        }
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate,
                                  params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public virtual T AddAndReturnEntity(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public virtual T AddWithCommit(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public virtual void Update(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void UpdateWithCommit(T entity)
        {
            Update(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public virtual async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
        public virtual void Commit()
        {
            _context.SaveChanges();
        }

        public void AddAll(IEnumerable<T> list)
        {
            _context.Set<T>().AddRange(list);
        }

        public void AddAllWithCommit(IEnumerable<T> list)
        {
            _context.Set<T>().AddRange(list);
            _context.SaveChanges();
        }

    }
}
