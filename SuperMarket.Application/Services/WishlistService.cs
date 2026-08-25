using System.Linq.Expressions;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Wishlist;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<Guid>> AddAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return Result<Guid>.Failure("UserId is required.");

        if (productId == Guid.Empty)
            return Result<Guid>.Failure("ProductId is required.");

        var existing = await _wishlistRepository.GetSingleAsync(
            w => w.UserId == userId && w.ProductId == productId,
            cancellationToken);

        if (existing is not null)
            return Result<Guid>.Success(existing.Id);

        var productExists = await _productRepository.ExistsAsync(
            p => p.Id == productId && p.IsActive,
            cancellationToken);

        if (!productExists)
            return Result<Guid>.Failure("Product not found.");

        WishlistItem item;

        try
        {
            item = new WishlistItem(userId, productId);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await _wishlistRepository.AddAsync(item, cancellationToken);
        await _wishlistRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> RemoveAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var item = await _wishlistRepository.GetSingleAsync(
            w => w.UserId == userId && w.ProductId == productId,
            cancellationToken);

        if (item is null)
            return Result.Success();

        item.SoftDelete(userId);

        await _wishlistRepository.UpdateAsync(item, cancellationToken);
        await _wishlistRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<PagedResult<WishlistItemDto>> GetByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var paging = Paging.Normalize(pageNumber, pageSize);

        var totalCount = await _wishlistRepository.CountAsync(
            w => w.UserId == userId,
            cancellationToken);

        var items = await _wishlistRepository.ListPagedAsync<DateTimeOffset>(
            predicate: w => w.UserId == userId,
            orderBy: w => w.CreatedDate,
            ascending: false,
            skip: paging.Skip,
            take: paging.Take,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<WishlistItem, object>>[] { w => w.Product });

        var dtos = items
            .Where(w => w.Product is not null)
            .Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductTitle = w.Product.Title.Value,
                ProductImageUrl = w.Product.ImageUrl,
                Price = w.Product.Price.Amount,
                IsInStock = w.Product.Stock.Value > 0,
                CreatedAt = w.CreatedDate
            })
            .ToList();

        return PagedResult<WishlistItemDto>.Success(
            dtos,
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }
}
