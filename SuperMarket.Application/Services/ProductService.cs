
using AutoMapper;
using FluentValidation;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<ProductCreateDto> _createValidator;
    private readonly IValidator<ProductUpdateDto> _updateValidator;

    public ProductService(
        IProductRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser,
        IValidator<ProductCreateDto> createValidator,
        IValidator<ProductUpdateDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    #region Helpers

    private Guid CurrentUserId => _currentUser.UserId;

    private async Task<Product?> GetProductOrNullAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithCategoryAsync(
            id,
            cancellationToken);
    }

    public async Task<PagedResult<ProductCustomerDto>> GetPagedForCustomerAsync(
        ProductCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var products = await _repository.GetCustomerProductsAsync(
            query.CategoryId,
            query.SearchTerm,
            query.OnlyInStock,
            query.MinPrice,
            query.MaxPrice,
            query.SortBy,
            skip,
            query.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountCustomerProductsAsync(
            query.CategoryId,
            query.SearchTerm,
            query.OnlyInStock,
            query.MinPrice,
            query.MaxPrice,
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<ProductCustomerDto>>(products);

        return PagedResult<ProductCustomerDto>.Success(
            dtos,
            query.PageNumber,
            query.PageSize,
            totalCount);
    }

    #endregion

    #region Admin

    public async Task<Result<Guid>> CreateAsync(
        ProductCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToFailureResult<Guid>();

        if (string.IsNullOrWhiteSpace(dto.Slug))
            return Result<Guid>.Failure("Slug is required.");

        var slugExists = await _repository.ExistsBySlugAsync(
            dto.Slug,
            cancellationToken: cancellationToken);

        if (slugExists)
            return Result<Guid>.Failure("Slug already exists.");

        var product = new Product(
            dto.Title,
            dto.Price,
            dto.Stock,
            dto.ImageUrl,
            dto.CategoryId,
            dto.Description,
            dto.Slug,
            dto.DisplayOrder);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }

    public async Task<Result> UpdateAsync(
        ProductUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToResult();

        var product = await GetProductOrNullAsync(
            dto.Id,
            cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        var slugExists = await _repository.ExistsBySlugAsync(
            dto.Slug,
            dto.Id,
            cancellationToken);

        if (slugExists)
            return Result.Failure("Slug already exists.");

        product.Update(
            dto.Title,
            dto.Price,
            dto.Stock,
            dto.ImageUrl,
            dto.CategoryId,
            dto.Description,
            dto.Slug,
            dto.DisplayOrder,
            CurrentUserId);

        product.SetActive(dto.IsActive, CurrentUserId);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        product.SoftDelete(CurrentUserId);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ProductAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null)
            return Result<ProductAdminDto>.Failure("Product not found.");

        return Result<ProductAdminDto>.Success(
            _mapper.Map<ProductAdminDto>(product));
    }

    public async Task<PagedResult<ProductAdminDto>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var paging = Paging.Normalize(pageNumber, pageSize);

        var totalCount = await _repository.CountAsync(
            cancellationToken: cancellationToken);

        var products = await _repository.ListAsync(
            orderBy: q => q.OrderBy(p => p.SortOrder.Value),
            skip: paging.Skip,
            take: paging.Take,
            cancellationToken: cancellationToken);

        var dtos =
            _mapper.Map<IReadOnlyList<ProductAdminDto>>(products);

        return PagedResult<ProductAdminDto>.Success(
            dtos,
            paging.PageNumber,
            paging.PageSize,
            totalCount);
    }

    public async Task<Result> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        product.SetActive(isActive, CurrentUserId);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> IncreaseStockAsync(
        Guid id,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            return Result.Failure(
                "Quantity must be greater than zero.");
        }

        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        product.IncreaseStock(quantity, CurrentUserId);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DecreaseStockAsync(
        Guid id,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            return Result.Failure(
                "Quantity must be greater than zero.");
        }

        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null)
            return Result.Failure("Product not found.");

        product.DecreaseStock(quantity, CurrentUserId);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Customer

    public async Task<Result<ProductCustomerDto>>
        GetByIdForCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrNullAsync(
            id,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            return Result<ProductCustomerDto>.Failure(
                "Product not found.");
        }

        var dto = _mapper.Map<ProductCustomerDto>(product);

        return Result<ProductCustomerDto>.Success(dto);
    }

    #endregion
}