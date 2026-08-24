
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Domain.Interfaces.Repositories;

/// <summary>
/// شمارش سبک فعالیت کاربر، بدون بارگذاری کامل گراف Order/Cart.
/// این نوع صرفاً برای Projectionهای سطح دیتابیس استفاده می‌شود.
/// </summary>
public sealed record UserActivityCounts(int OrderCount, int CartItemCount);

public interface IUserRepository : IRepository<User, Guid>
{
    #region Get

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailWithDetailsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User?> GetWithOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetWithCartsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetFullGraphAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// کاربر را حتی در صورت Soft-Delete شدن برمی‌گرداند.
    /// پیاده‌سازی این متد باید صراحتاً Global Query Filter مربوط به IsDeleted
    /// را نادیده بگیرد (IgnoreQueryFilters)؛ در غیر این صورت قابلیت Restore
    /// همواره با خطای «User not found» مواجه می‌شود.
    /// </summary>
    Task<User?> GetIncludingDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    #endregion

    #region List

    Task<IReadOnlyList<User>> ListActiveAsync(
        Expression<Func<User, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListByRoleAsync(
        UserRole role,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListPagedAsync<TKey>(
        Expression<Func<User, bool>>? predicate = null,
        Expression<Func<User, TKey>>? orderBy = null,
        bool ascending = true,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default,
        params Expression<Func<User, object>>[] includes);

    #endregion

    #region Search

    Task<IReadOnlyList<User>> SearchAsync(
        string searchTerm,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    #endregion

    #region Count

    Task<int> CountAsync(
        Expression<Func<User, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<int> CountByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تعداد سفارش‌ها و آیتم‌های سبد خرید را برای مجموعه‌ای از کاربران،
    /// در یک Query سبک سطح دیتابیس (بدون Include کامل گراف Order/Cart) برمی‌گرداند.
    /// کاربرانی که در نتیجه وجود ندارند باید توسط فراخواننده با مقدار صفر در نظر گرفته شوند.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, UserActivityCounts>> GetOrderAndCartCountsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validation

    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);

    #endregion
}
