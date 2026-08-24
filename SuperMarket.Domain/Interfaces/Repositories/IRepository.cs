

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class
        where TKey : notnull
    {
        IQueryable<TEntity> Query();

        IQueryable<TEntity> Query(
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsync(
            TKey id,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetSingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> ListAllAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> ListWhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> ListPagedAsync<TOrderKey>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, TOrderKey>>? orderBy = null,
            bool ascending = true,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        void Attach(TEntity entity);

        void Detach(TEntity entity);

        Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
