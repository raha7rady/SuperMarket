

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories;

public sealed class UserRepository
    : Repository<User, Guid>, IUserRepository
{
    public UserRepository(
        SuperMarketDbContext dbContext)
        : base(dbContext)
    {
    }


    private IQueryable<User> ActiveUsers()
    {
        return ReadOnlyQuery()
            .Where(u => !u.IsDeleted);
    }

    #region Get

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        email = email.Trim().ToLowerInvariant();

        return await ActiveUsers()

            .FirstOrDefaultAsync(
                u =>
                     u.Email.Value == email,
                cancellationToken);
    }

    public async Task<User?> GetByEmailWithDetailsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        email = email.Trim().ToLowerInvariant();

        return await ActiveUsers()

            .AsSplitQuery()
            .Include(u => u.Orders)
            .Include(u => u.Carts)
            .FirstOrDefaultAsync(
                u =>
                     u.Email.Value == email,
                cancellationToken);
    }

    public async Task<User?> GetWithOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await ActiveUsers()

            .AsSplitQuery()
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(
                u =>
                     u.Id == userId,
                cancellationToken);
    }

    public async Task<User?> GetWithCartsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await ActiveUsers()

            .AsSplitQuery()
            .Include(u => u.Carts)
                .ThenInclude(c => c.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(
                u =>
                     u.Id == userId,
                cancellationToken);
    }

    public async Task<User?> GetFullGraphAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await ActiveUsers()

            .AsSplitQuery()
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .Include(u => u.Carts)
                .ThenInclude(c => c.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(
                u =>
                     u.Id == userId,
                cancellationToken);
    }

    public async Task<User?> GetIncludingDeletedByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await ReadOnlyQuery()
            .FirstOrDefaultAsync(
                u => u.Id == id,
                cancellationToken);
    }

    #endregion

    #region List

    public async Task<IReadOnlyList<User>> ListActiveAsync(
        Expression<Func<User, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = ActiveUsers();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListByRoleAsync(
        UserRole role,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        return await ActiveUsers()

            .Where(u => u.Role == role)
            .OrderBy(u => u.Name.FirstName)
            .ThenBy(u => u.Name.LastName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListPagedAsync<TKey>(
        Expression<Func<User, bool>>? predicate = null,
        Expression<Func<User, TKey>>? orderBy = null,
        bool ascending = true,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default,
        params Expression<Func<User, object>>[] includes)
    {
        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        IQueryable<User> query = ActiveUsers();

        if (includes.Length > 0)
        {
            query = query.AsSplitQuery();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }


        if (orderBy is not null)
        {
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);
        }
        else
        {
            query = query
                .OrderBy(u => u.Name.FirstName)
                .ThenBy(u => u.Name.LastName);
        }

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Search

    public async Task<IReadOnlyList<User>> SearchAsync(
        string searchTerm,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {

        searchTerm = searchTerm.Trim();

        if (searchTerm.Length == 0)
            return Array.Empty<User>();

        skip = Math.Max(skip, 0);
        take = Math.Clamp(take, 1, 100);

        return await ActiveUsers()
            .Where(u =>
                (
                    EF.Functions.Like(
                        u.Email.Value,
                        $"%{searchTerm}%")
                    ||

                    EF.Functions.Like(
                        u.Name.FirstName,
                        $"%{searchTerm}%")
                    ||

                    EF.Functions.Like(
                        u.Name.LastName,
                        $"%{searchTerm}%")
                ))
            .OrderBy(u => u.Name.FirstName)
            .ThenBy(u => u.Name.LastName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Count

    public async Task<int> CountAsync(
        Expression<Func<User, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = ActiveUsers();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<int> CountByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        return await ActiveUsers()
            .Where(u =>
                u.Role == role)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, UserActivityCounts>> GetOrderAndCartCountsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var ids = userIds.Distinct().ToList();

        if (ids.Count == 0)
        {
            return new Dictionary<Guid, UserActivityCounts>();
        }

        var counts = await ActiveUsers()
            .Where(u => ids.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                OrderCount = u.Orders.Count,
                CartItemCount = u.Carts.SelectMany(c => c.Items).Count()
            })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(
            x => x.Id,
            x => new UserActivityCounts(x.OrderCount, x.CartItemCount));
    }

    #endregion

    #region Validation

    public async Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeUserId = null,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        email = email.Trim().ToLowerInvariant();

        // عمداً از ReadOnlyQuery() به‌جای ActiveUsers() استفاده می‌شود:
        // Index یکتای UserEmail در سطح دیتابیس صرف‌نظر از IsDeleted اعمال
        // می‌شود. اگر این متد فقط کاربران فعال را بررسی کند، تلاش برای
        // ثبت‌نام/ایجاد کاربر با ایمیلِ یک کاربر Soft-Delete شده به‌جای یک
        // Result.Failure تمیز، با خطای Unique Constraint دیتابیس مواجه
        // می‌شود.
        IQueryable<User> query = ReadOnlyQuery()
            .Where(u =>
                u.Email.Value == email);

        if (excludeUserId.HasValue)
        {
            query = query.Where(
                u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    #endregion
}
