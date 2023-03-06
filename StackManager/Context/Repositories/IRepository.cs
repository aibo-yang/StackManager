using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using StackManager.Context.Domain;

namespace StackManager.Repositories
{
    interface IRepository<T> where T : IEntity
    {
        Task<T> FindAsync(Guid id);

        IQueryable<T> TrackingQuery();

        IQueryable<T> NoTrackingQuery();

        Task<IPagedList<T>> GetPagedListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            CancellationToken cancellationToken = default(CancellationToken),
            bool disableTracking = false,
            bool ignoreQueryFilters = false);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> selector = null);

        Task<EntityEntry<T>> AddAsync(T entity);

        Task AddRangeAsync(params T[] entities);

        Task<EntityEntry<T>> UpdateAsync(T entity);

        Task<bool> TryUpdateAsync(T entity);

        Task UpdatePartialsAsync(T entity, params Expression<Func<T, object>>[] updatedProperties);

        Task DeleteAsync(T entity);

        Task<TReturn> MaxAsync<TReturn>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TReturn>> selector = null);
    }
}
