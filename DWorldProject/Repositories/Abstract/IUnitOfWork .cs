using System;
using System.Threading.Tasks;

namespace DWorldProject.Repositories.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IBlogPostRepository BlogPosts { get; }
        Task<int> CommitAsync();
    }
}
