using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Reviews;

namespace SuperMarket.Application.Interfaces.Services;

public interface IReviewService
{
    Task<Result<Guid>> CreateAsync(
        Guid userId,
        ReviewCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        Guid id,
        Guid userId,
        ReviewUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ReviewDto>> GetByProductAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<ReviewSummaryDto>> GetSummaryAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
