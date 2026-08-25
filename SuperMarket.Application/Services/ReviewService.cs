using System.Linq.Expressions;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Reviews;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> CreateAsync(
        Guid userId,
        ReviewCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return Result<Guid>.Failure("UserId is required.");

        if (dto.ProductId == Guid.Empty)
            return Result<Guid>.Failure("ProductId is required.");

        var productExists = await _productRepository.ExistsAsync(
            p => p.Id == dto.ProductId && p.IsActive && !p.IsDeleted,
            cancellationToken);

        if (!productExists)
            return Result<Guid>.Failure("Product not found.");

        var userExists = await _userRepository.ExistsAsync(
            u => u.Id == userId && !u.IsDeleted,
            cancellationToken);

        if (!userExists)
            return Result<Guid>.Failure("User not found.");

        var alreadyReviewed = await _reviewRepository.ExistsAsync(
            r => r.ProductId == dto.ProductId && r.UserId == userId && !r.IsDeleted,
            cancellationToken);

        if (alreadyReviewed)
            return Result<Guid>.Failure("You have already reviewed this product.");

        Review review;

        try
        {
            review = new Review(dto.ProductId, userId, dto.Rating, dto.Comment);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(review.Id);
    }

    public async Task<Result> UpdateAsync(
        Guid id,
        Guid userId,
        ReviewUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);

        if (review is null || review.UserId != userId)
            return Result.Failure("Review not found.");

        try
        {
            review.Update(dto.Rating, dto.Comment, userId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);

        if (review is null || review.UserId != userId)
            return Result.Failure("Review not found.");

        review.SoftDelete(userId);

        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<PagedResult<ReviewDto>> GetByProductAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var paging = Paging.Normalize(pageNumber, pageSize);

        var totalCount = await _reviewRepository.CountAsync(
            r => r.ProductId == productId && !r.IsDeleted,
            cancellationToken);

        var reviews = await _reviewRepository.ListPagedAsync<DateTimeOffset>(
            predicate: r => r.ProductId == productId && !r.IsDeleted,
            orderBy: r => r.CreatedDate,
            ascending: false,
            skip: paging.Skip,
            take: paging.Take,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Review, object>>[] { r => r.User });

        var dtos = reviews
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                ReviewerName = r.User?.Name is not null ? r.User.Name.ToString() : string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedDate
            })
            .ToList();

        return PagedResult<ReviewDto>.Success(
            dtos,
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }

    public async Task<Result<ReviewSummaryDto>> GetSummaryAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            return Result<ReviewSummaryDto>.Failure("ProductId is required.");

        var (average, count) = await _reviewRepository.GetRatingSummaryAsync(
            productId,
            cancellationToken);

        return Result<ReviewSummaryDto>.Success(new ReviewSummaryDto
        {
            AverageRating = Math.Round(average, 1),
            ReviewCount = count
        });
    }
}
