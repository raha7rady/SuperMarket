
using System.Linq.Expressions;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
namespace SuperMarket.Domain.Interfaces.Repositories;

public interface IProductRepository
    : IRepository<Product, Guid>
{
    #region Single Queries

    Task<Product?> GetByIdWithCategoryAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Product?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    #endregion

    #region Exists

    Task<bool> ExistsByTitleAsync(
        string title,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Generic Queries

    Task<int> CountAsync(
        Expression<Func<Product, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListAsync(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>,
        IOrderedQueryable<Product>>? orderBy = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    #endregion

    #region Admin Queries

    Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListActivePagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListLowStockAsync(
        int threshold,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    #endregion

    #region Customer Queries

    Task<IReadOnlyList<Product>> GetCustomerProductsAsync(
        Guid? categoryId,
        string? searchTerm,
        bool onlyInStock,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortBy sortBy,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountCustomerProductsAsync(
        Guid? categoryId,
        string? searchTerm,
        bool onlyInStock,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken cancellationToken = default);

    #endregion

    #region Stock

    Task<bool> TryDecreaseStockAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken = default);

    #endregion
}