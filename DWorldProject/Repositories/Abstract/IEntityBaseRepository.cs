using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DWorldProject.Repositories.Abstract
{
    public interface IEntityBaseRepository<T> where T : class, new()
    {
        IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AllIncludingAsQueryable(params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll();
        Task<int> CountAsync();
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Add(T entity);
        T AddWithCommit(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteWhere(Expression<Func<T, bool>> predicate);
        Task CommitAsync();
        void Commit();
        void AddAll(IEnumerable<T> list);
        void AddAllWithCommit(IEnumerable<T> list);
        void UpdateWithCommit(T entity);
        T AddAndReturnEntity(T entity);
        IQueryable<T> AsQueryable();
    }
}
