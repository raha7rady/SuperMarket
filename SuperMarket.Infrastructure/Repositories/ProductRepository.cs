
using Microsoft.EntityFrameworkCore;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;
using System.Linq.Expressions;

namespace SuperMarket.Infrastructure.Repositories;

public sealed class ProductRepository
    : Repository<Product, Guid>,
      IProductRepository
{
    private const int DefaultTake = 20;

    public ProductRepository(SuperMarketDbContext dbContext)
        : base(dbContext)
    {
    }

    #region Base Query

    private IQueryable<Product> BaseQuery(bool asNoTracking = true)
    {
        var query = _dbSet.Where(p => !p.IsDeleted);

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private static IQueryable<Product> IncludeCategory(IQueryable<Product> query)
        => query.Include(p => p.Category);

    #endregion

    #region Normalization

    private static int NormalizeSkip(int skip)
        => Math.Max(skip, 0);

    private static int NormalizeTake(int take)
        => take <= 0 ? DefaultTake : take;

    #endregion

    #region Single Queries

    public async Task<Product?> GetByIdWithCategoryAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await IncludeCategory(BaseQuery())
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        slug = slug.Trim().ToLowerInvariant();

        return await IncludeCategory(BaseQuery())
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    #endregion

    #region Exists

    public async Task<bool> ExistsByTitleAsync(
        string title,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;

        title = title.Trim();

        var query = BaseQuery()
            .Where(p => p.Title.Value == title);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        slug = slug.Trim().ToLowerInvariant();

        var query = BaseQuery()
            .Where(p => p.Slug == slug);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    #endregion

    #region Generic Queries

    public async Task<int> CountAsync(
        Expression<Func<Product, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = BaseQuery();

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        skip = NormalizeSkip(skip);
        take = NormalizeTake(take);

        var query = IncludeCategory(BaseQuery());

        if (predicate is not null)
            query = query.Where(predicate);

        if (orderBy is not null)
            query = orderBy(query);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Admin Queries

    public async Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await BaseQuery()
            .CountAsync(p => p.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListActivePagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        skip = NormalizeSkip(skip);
        take = NormalizeTake(take);

        return await IncludeCategory(BaseQuery())
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder.Value)
            .ThenBy(p => p.Title.Value)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListLowStockAsync(
        int threshold,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        skip = NormalizeSkip(skip);
        take = NormalizeTake(take);

        return await IncludeCategory(BaseQuery())
            .Where(p => p.IsActive && p.Stock.Value <= threshold)
            .OrderBy(p => p.Stock.Value)
            .ThenBy(p => p.Title.Value)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Customer Queries

    public async Task<IReadOnlyList<Product>> GetCustomerProductsAsync(
        Guid? categoryId,
        string? searchTerm,
        bool onlyInStock,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortBy sortBy,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        skip = NormalizeSkip(skip);
        take = NormalizeTake(take);

        var query = BuildCustomerQuery(
            categoryId,
            searchTerm,
            onlyInStock,
            minPrice,
            maxPrice);

        query = ApplySorting(query, sortBy);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountCustomerProductsAsync(
        Guid? categoryId,
        string? searchTerm,
        bool onlyInStock,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken cancellationToken = default)
    {
        return await BuildCustomerQuery(
                categoryId,
                searchTerm,
                onlyInStock,
                minPrice,
                maxPrice)
            .CountAsync(cancellationToken);
    }

    #endregion

    #region Private Query Builder (Refactored)

    private IQueryable<Product> BuildCustomerQuery(
        Guid? categoryId,
        string? searchTerm,
        bool onlyInStock,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = IncludeCategory(BaseQuery())
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (onlyInStock)
            query = query.Where(p => p.Stock.Value > 0);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(p =>
                EF.Functions.Like(p.Title.Value, $"%{searchTerm}%") ||
                EF.Functions.Like(p.Description ?? "", $"%{searchTerm}%"));
        }

        return query;
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        ProductSortBy sortBy)
    {
        return sortBy switch
        {
            ProductSortBy.PriceLowToHigh =>
                query.OrderBy(p => p.Price.Amount),

            ProductSortBy.PriceHighToLow =>
                query.OrderByDescending(p => p.Price.Amount),

            ProductSortBy.NameAscending =>
                query.OrderBy(p => p.Title.Value),

            ProductSortBy.Newest =>
                query.OrderByDescending(p => p.CreatedDate),

            _ =>
                query.OrderBy(p => p.SortOrder.Value)
        };
    }

    #endregion

    #region Stock

    public async Task<bool> TryDecreaseStockAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var affected = await _dbSet
            .Where(p => p.Id == productId && !p.IsDeleted && p.Stock.Value >= quantity)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.Stock.Value, p => p.Stock.Value - quantity),
                cancellationToken);

        return affected > 0;
    }

    #endregion
}