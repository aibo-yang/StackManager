using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StackManager.Context.Domain;

namespace StackManager.Repositories
{
    interface IUnitOfWork : IDisposable
    {
        void ClearDbContext();

        IRepository<T> GetRepository<T>() where T : IEntity;

        Task<bool> SaveChangesAsync(Func<EntityEntry, Task<bool>> haveUserResolveConcurrency = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}
