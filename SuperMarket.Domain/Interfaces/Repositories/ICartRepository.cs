


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories;

public interface ICartRepository
    : IRepository<Cart, Guid>
{
    Task<Cart?> GetActiveCartByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetActiveCartWithItemsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetWithItemsAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveCartAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cart>> ListActiveAsync(
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cart>> ListByUserAsync(
        Guid userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cart>> ListPagedWithIncludesAsync<TOrderKey>(
        Expression<Func<Cart, bool>>? predicate = null,
        Expression<Func<Cart, TOrderKey>>? orderBy = null,
        bool ascending = true,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default,
        params Expression<Func<Cart, object>>[] includes);

    Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default);

    Task<int> CountByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(
        Guid cartId,
        Guid deletedBy,
        CancellationToken cancellationToken = default);

    Task RestoreAsync(
        Guid cartId,
        Guid restoredBy,
        CancellationToken cancellationToken = default);

    // Acquires a row lock on the user's active cart for the current transaction,
    // to serialize concurrent checkout attempts.
    Task LockForCheckoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}