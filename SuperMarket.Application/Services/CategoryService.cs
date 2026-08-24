

using AutoMapper;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Categories;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using System.Linq.Expressions;


namespace SuperMarket.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    #region Admin Operations

    public async Task<Result<Guid>> CreateAsync(
        CategoryCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto is null)
        {
            return Result<Guid>.Failure(
                "Request cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<Guid>.Failure(
                "Title is required.");
        }

        var exists = await _categoryRepository
            .ExistsByTitleAsync(
                dto.Title.Trim(),
                cancellationToken: cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                "Category title already exists.");
        }

        var category = new Category(
            dto.Title.Trim(),
            dto.DisplayOrder);

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        await _categoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(category.Id);
    }

    public async Task<Result> UpdateAsync(
        CategoryUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto is null)
        {
            return Result.Failure(
                "Request cannot be null.");
        }

        if (dto.Id == Guid.Empty)
        {
            return Result.Failure(
                "Category id is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result.Failure(
                "Title is required.");
        }

        var category = await _categoryRepository
            .GetByIdAsync(
                dto.Id,
                cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                "Category not found.");
        }

        var duplicateTitle = await _categoryRepository
            .ExistsByTitleAsync(
                dto.Title.Trim(),
                dto.Id,
                cancellationToken);

        if (duplicateTitle)
        {
            return Result.Failure(
                "Category title already exists.");
        }

        category.Update(
            dto.Title.Trim(),
            dto.DisplayOrder,
            _currentUser.UserId);

        category.SetActive(
            dto.IsActive,
            _currentUser.UserId);

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        await _categoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure(
                "Category id is required.");
        }

        var category = await _categoryRepository
            .GetByIdAsync(
                id,
                cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                "Category not found.");
        }

        category.SoftDelete(
            _currentUser.UserId);

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        await _categoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure(
                "Category id is required.");
        }

        var category = await _categoryRepository
            .GetByIdAsync(
                id,
                cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                "Category not found.");
        }

        category.SetActive(
            isActive,
            _currentUser.UserId);

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        await _categoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CategoryAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<CategoryAdminDto>.Failure(
                "Category id is required.");
        }

        var category = await _categoryRepository
            .GetWithProductsAsync(
                id,
                cancellationToken);

        if (category is null)
        {
            return Result<CategoryAdminDto>.Failure(
                "Category not found.");
        }

        var dto = _mapper.Map<CategoryAdminDto>(
            category);

        dto.ProductCount = category.Products.Count;

        return Result<CategoryAdminDto>.Success(dto);
    }

    public async Task<PagedResult<CategoryAdminDto>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0
            ? 1
            : pageNumber;

        pageSize = pageSize <= 0
            ? 10
            : pageSize;

        var totalCount = await _categoryRepository
            .CountActiveAsync(
                cancellationToken: cancellationToken);

        var categories = await _categoryRepository
            .ListActivePagedAsync<int>(
                orderBy: c => c.DisplayOrder,
                ascending: true,
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                cancellationToken: cancellationToken,
                includes: new Expression<Func<Category, object>>[]
                {
            c => c.Products
                });

        var dtos = categories
            .Select(category =>
            {
                var dto = _mapper.Map<CategoryAdminDto>(
                    category);

                dto.ProductCount =
                    category.Products.Count;

                return dto;
            })
            .ToList();

        return PagedResult<CategoryAdminDto>.Success(
            dtos,
            pageNumber,
            pageSize,
            totalCount);
    }

    public async Task<PagedResult<CategoryCustomerDto>> GetPagedForCustomerAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0
            ? 1
            : pageNumber;

        pageSize = pageSize <= 0
            ? 10
            : pageSize;

        var totalCount = await _categoryRepository
            .CountActiveAsync(
                c => c.IsActive,
                cancellationToken);

        var categories = await _categoryRepository
            .ListActivePagedAsync<int>(
                predicate: c => c.IsActive,
                orderBy: c => c.DisplayOrder,
                ascending: true,
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                cancellationToken: cancellationToken);

        var dtos = _mapper.Map<
            List<CategoryCustomerDto>>(
            categories);

        return PagedResult<CategoryCustomerDto>.Success(
            dtos,
            pageNumber,
            pageSize,
            totalCount);
    }

    public async Task<Result<IReadOnlyList<CategoryLookupDto>>> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository
            .ListActiveAsync(
                c => c.IsActive,
                cancellationToken);

        var dtos = categories
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryLookupDto
            {
                Id = c.Id,
                Name = c.Title
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<CategoryLookupDto>>
            .Success(dtos);
    }

    #endregion
}
