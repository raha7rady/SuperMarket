
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Products.Queries;

namespace SuperMarket.Application.Interfaces.Services;

public interface IProductService
{
    #region Admin

    Task<Result<Guid>> CreateAsync(
        ProductCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        ProductUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateCatalogDetailsAsync(
        Guid id,
        ProductCatalogDetailsDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<ProductAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductAdminDto>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<Result> IncreaseStockAsync(
        Guid id,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<Result> DecreaseStockAsync(
        Guid id,
        int quantity,
        CancellationToken cancellationToken = default);

    #endregion

    #region Customer

    Task<Result<ProductCustomerDto>> GetByIdForCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductCustomerDto>> GetPagedForCustomerAsync(
        ProductCustomerQuery query,
        CancellationToken cancellationToken = default);

    #endregion
}
