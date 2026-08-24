
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Orders;

namespace SuperMarket.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result<Guid>> CreateAsync(
            CreateOrderDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result<OrderAdminDto>> GetByIdForAdminAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<OrderCustomerDto>> GetByIdForCustomerAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PagedResult<OrderAdminDto>> GetPagedForAdminAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResult<OrderCustomerDto>> GetPagedForCustomerAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result> AddItemAsync(
            Guid orderId,
            OrderItemDto dto,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> RemoveItemAsync(
            Guid orderId,
            Guid productId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> ChangeItemQuantityAsync(
            Guid orderId,
            Guid productId,
            int quantity,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> MarkAsPaidAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> MarkAsShippedAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> MarkAsDeliveredAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> CancelAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> MarkAsRefundedAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> RestoreAsync(
            Guid orderId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

    }
}
