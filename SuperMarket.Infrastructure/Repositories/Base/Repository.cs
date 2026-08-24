using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;

namespace SuperMarket.Infrastructure.Repositories.Base;

public class Repository<TEntity, TKey>
    : IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    protected readonly SuperMarketDbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(
        SuperMarketDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    #region Query

    public virtual IQueryable<TEntity> Query()
    {
        return _dbSet;
    }

    public virtual IQueryable<TEntity> Query(
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet
            .AsSplitQuery();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    protected virtual IQueryable<TEntity> ReadOnlyQuery()
    {
        return _dbSet.AsNoTracking();
    }

    #endregion

    #region Get

    public virtual async Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    public virtual async Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await ReadOnlyQuery()
            .FirstOrDefaultAsync(
                predicate,
                cancellationToken);
    }

    #endregion

    #region List

    public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await ReadOnlyQuery()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await ReadOnlyQuery()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListPagedAsync<TOrderKey>(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TOrderKey>>? orderBy = null,
        bool ascending = true,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        skip = Math.Max(0, skip);
        take = Math.Clamp(take, 1, 100);

        IQueryable<TEntity> query = ReadOnlyQuery()
            .AsSplitQuery();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (orderBy is not null)
        {
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);
        }

        query = query
            .Skip(skip)
            .Take(take);

        return await query.ToListAsync(cancellationToken);
    }

    #endregion

    #region Commands

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(
            entity,
            cancellationToken);
    }

    public virtual Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = _dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    #endregion

    #region Tracking

    public virtual void Attach(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Attach(entity);
    }

    public virtual void Detach(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbContext.Entry(entity).State =
            EntityState.Detached;
    }

    #endregion

    #region Aggregates

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await ReadOnlyQuery()
                .CountAsync(cancellationToken)
            : await ReadOnlyQuery()
                .CountAsync(
                    predicate,
                    cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await ReadOnlyQuery()
            .AnyAsync(
                predicate,
                cancellationToken);
    }

    #endregion

    #region UnitOfWork

    public virtual async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .SaveChangesAsync(cancellationToken);
    }

    #endregion
}
