


using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories;

public sealed class CartRepository
    : Repository<Cart, Guid>,
      ICartRepository
{
    public CartRepository(
        SuperMarketDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
    }


    #region User-Based Access

    public async Task<Cart?> GetActiveCartByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return null;

        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => !c.IsDeleted &&
                     c.UserId == userId,
                cancellationToken);
    }

    public async Task<Cart?> GetActiveCartWithItemsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return null;

        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(
                c => !c.IsDeleted &&
                     c.UserId == userId,
                cancellationToken);
    }

    public async Task<Cart?> GetWithItemsAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        if (cartId == Guid.Empty)
            return null;

        return await _dbSet
            .AsTracking()
            .AsSplitQuery()
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(
                c => c.Id == cartId &&
                     !c.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> HasActiveCartAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return false;

        return await _dbSet.AnyAsync(
            c => !c.IsDeleted &&
                 c.UserId == userId,
            cancellationToken);
    }

    #endregion

    #region Single Entity

    public async Task<Cart?> FirstOrDefaultWithIncludesAsync(
        Expression<Func<Cart, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<Cart, object>>[] includes)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        IQueryable<Cart> query = _dbSet
            .AsNoTracking()
            .AsSplitQuery();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(
            predicate,
            cancellationToken);
    }

    #endregion

    #region Listing

    public async Task<IReadOnlyList<Cart>> ListActiveAsync(
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        return await _dbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cart>> ListByUserAsync(
        Guid userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        return await _dbSet
            .AsNoTracking()
            .Where(c =>
                !c.IsDeleted &&
                c.UserId == userId)
            .OrderByDescending(c => c.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cart>> ListPagedWithIncludesAsync<TOrderKey>(
        Expression<Func<Cart, bool>>? predicate = null,
        Expression<Func<Cart, TOrderKey>>? orderBy = null,
        bool ascending = true,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default,
        params Expression<Func<Cart, object>>[] includes)
    {
        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        IQueryable<Cart> query = _dbSet
            .AsNoTracking()
            .AsSplitQuery();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = orderBy is null
            ? query.OrderByDescending(c => c.CreatedDate)
            : ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Aggregates

    public async Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(
            c => !c.IsDeleted,
            cancellationToken);
    }

    public async Task<int> CountByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return 0;

        return await _dbSet.CountAsync(
            c => !c.IsDeleted &&
                 c.UserId == userId,
            cancellationToken);
    }

    #endregion

    #region Soft Delete

    public async Task SoftDeleteAsync(
        Guid cartId,
        Guid deletedBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(
            cartId,
            cancellationToken);

        if (cart is null || cart.IsDeleted)
            return;

        cart.SoftDelete(deletedBy);
        _dbContext.Entry(cart).State = EntityState.Modified;
    }

    public async Task RestoreAsync(
        Guid cartId,
        Guid restoredBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(
            cartId,
            cancellationToken);

        if (cart is null || !cart.IsDeleted)
            return;

        cart.Restore(restoredBy);
        _dbContext.Entry(cart).State = EntityState.Modified;
    }

    public async Task LockForCheckoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT TOP (1) Id FROM Carts WITH (UPDLOCK, ROWLOCK) WHERE UserId = {userId} AND IsDeleted = 0",
            cancellationToken);
    }

    #endregion
}