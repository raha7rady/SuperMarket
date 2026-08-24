
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        #region User-Based Access

        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(
            Guid userId,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetOrdersByUserIdWithItemsAsync(
            Guid userId,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status-Based Access

        Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(
            OrderStatus status,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetOrdersByPaymentStatusAsync(
            PaymentStatus status,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        #endregion

        #region Aggregate Root Access

        Task<Order?> GetOrderWithItemsAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<Order?> GetFullGraphAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Listing / Paging

        Task<IReadOnlyList<Order>> ListActiveAsync(
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> ListPagedWithIncludesAsync<TOrderKey>(
            Expression<Func<Order, bool>>? predicate = null,
            Expression<Func<Order, TOrderKey>>? orderBy = null,
            bool ascending = true,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default,
            params Expression<Func<Order, object>>[] includes);

        #endregion

        #region Aggregates / Counts

        Task<int> CountByUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<int> CountByStatusAsync(
            OrderStatus status,
            CancellationToken cancellationToken = default);

        #endregion

        #region Soft Delete Support

        Task SoftDeleteAsync(
            Guid orderId,
            Guid deletedBy,
            CancellationToken cancellationToken = default);

        Task RestoreAsync(
            Guid orderId,
            Guid restoredBy,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
