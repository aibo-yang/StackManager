using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using StackManager.Context.Domain;

namespace StackManager.Repositories
{
    class Repository<T> : IRepository<T> where T : IEntity
    {
        protected readonly DbContext dbContext;
        protected readonly DbSet<T> dbSet;

        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.dbSet = this.dbContext.Set<T>();
        }

        public async Task<T> FindAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<T> TrackingQuery()
        {
            return dbSet;
        }

        public IQueryable<T> NoTrackingQuery()
        {
            return dbSet.AsNoTracking();
        }

        public virtual Task<IPagedList<T>> GetPagedListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            CancellationToken cancellationToken = default(CancellationToken),
            bool disableTracking = false,
            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
            else
            {
                return query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await dbSet.CountAsync();
            }
            else
            {
                return await dbSet.CountAsync(predicate);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> selector = null)
        {
            if (selector == null)
            {
                return await dbSet.AnyAsync();
            }
            else
            {
                return await dbSet.AnyAsync(selector);
            }
        }

        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(params T[] entities)
        {
            if (entities == null && entities.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            await dbSet.AddRangeAsync(entities);
        }

        public async Task<EntityEntry<T>> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return await Task.Run(() => dbSet.Update(entity));
        }

        public async Task<bool> TryUpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Task.FromResult(true);
        }

        public async Task UpdatePartialsAsync(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await Task.Run(() =>
            {
                var entityEntry = dbContext.Entry(entity);
                if (updatedProperties.Any())
                {
                    foreach (var property in updatedProperties)
                    {
                        entityEntry.Property(property).IsModified = true;
                    }
                }
                else
                {
                    foreach (var propertyName in entityEntry.OriginalValues.Properties.Select(x => x.Name))
                    {
                        if (!object.Equals(entityEntry.Property(propertyName).OriginalValue, entityEntry.Property(propertyName).CurrentValue))
                        {
                            entityEntry.Property(propertyName).IsModified = true;
                        }
                    }
                }
            });
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Task.Run(() => dbSet.Remove(entity));
        }

        public async Task<TReturn> MaxAsync<TReturn>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TReturn>> selector = null)
        {
            if (predicate == null)
            {
                return await dbSet.MaxAsync(selector);
            }
            else
            {
                return dbSet.Where(predicate).Max(selector);
            }
        }
    }
}
