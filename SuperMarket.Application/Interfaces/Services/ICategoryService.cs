
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Categories;

namespace SuperMarket.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<Guid>> CreateAsync(
        CategoryCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        CategoryUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateCatalogDetailsAsync(
        Guid id,
        CategoryCatalogDetailsDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<CategoryAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<CategoryAdminDto>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<PagedResult<CategoryCustomerDto>> GetPagedForCustomerAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<CategoryLookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default);
}