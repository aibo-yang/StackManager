using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using StackManager.Context.Domain;

namespace StackManager.Repositories
{
    class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private bool disposed = false;
        protected readonly DbContext dbContext;
        protected readonly ILogger<UnitOfWork<TDbContext>> logger;

        public UnitOfWork(TDbContext dbContext, ILogger<UnitOfWork<TDbContext>> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger;
        }

        public async Task<bool> SaveChangesAsync(Func<EntityEntry, Task<bool>> userResolveConcurrency = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var saved = true;
            var saveContinued = false;

            do
            {
                saved = true;
                saveContinued = false;
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saved = false;
                    logger.LogError(ex, $"{memberName} {sourceFilePath} {sourceLineNumber} => {nameof(SaveChangesAsync)} Failed");

                    if (userResolveConcurrency != null)
                    {
                        saveContinued = true;
                        // 并发冲突
                        var resolvedResult = false;
                        try
                        {
                            resolvedResult = await userResolveConcurrency(ex.Entries.Single());
                        }
                        catch (Exception ex2)
                        {
                            logger.LogError(ex2, $"{memberName} {sourceFilePath} {sourceLineNumber} => {nameof(userResolveConcurrency)} Failed");
                        }
                        finally
                        {
                            if (!resolvedResult)
                            {
                                // 终止执行
                                saveContinued = false;
                            }
                        }
                    }
                }
                catch (Exception ex1)
                {
                    saved = false;
                    logger.LogError(ex1, $"{memberName} {sourceFilePath} {sourceLineNumber} => {nameof(SaveChangesAsync)} Failed");
                }
            } while (saveContinued);

            return saved;
        }

        public IRepository<T> GetRepository<T>() where T : IEntity
        {
            return (IRepository<T>)new Repository<T>(dbContext);
        }

        public void ClearDbContext()
        {
            dbContext.ChangeTracker.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            disposed = true;
        }
    }
}
