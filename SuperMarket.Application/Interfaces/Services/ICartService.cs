
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Cart;

namespace SuperMarket.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<Result<Guid>> CreateAsync(
            CartCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<Result<CartAdminDto>> GetByIdForAdminAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResult<CartAdminDto>> GetPagedForAdminAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<CartCustomerDto>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Result> AddItemAsync(
            Guid cartId,
            CartItemDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateItemAsync(
            Guid cartId,
            CartUpdateItemDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> RemoveItemAsync(
            Guid cartId,
            Guid productId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> ClearAsync(
            Guid cartId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid cartId,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        Task<Result> RestoreAsync(
            Guid cartId,
            Guid performedBy,
            CancellationToken cancellationToken = default);
    }
}
