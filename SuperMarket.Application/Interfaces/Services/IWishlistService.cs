using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Wishlist;

namespace SuperMarket.Application.Interfaces.Services;

public interface IWishlistService
{
    Task<Result<Guid>> AddAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<WishlistItemDto>> GetByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
