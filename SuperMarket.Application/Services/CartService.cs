

using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Cart;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CartService> _logger;
    private readonly IValidator<CartCreateDto> _createValidator;
    private readonly IValidator<CartItemDto> _addItemValidator;
    private readonly IValidator<CartUpdateItemDto> _updateItemValidator;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<CartService> logger,
        IValidator<CartCreateDto> createValidator,
        IValidator<CartItemDto> addItemValidator,
        IValidator<CartUpdateItemDto> updateItemValidator)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _addItemValidator = addItemValidator;
        _updateItemValidator = updateItemValidator;
    }

    #region Create

    public async Task<Result<Guid>> CreateAsync(
        CartCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto is null)
            return Result<Guid>.Failure("Request cannot be null.");

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToFailureResult<Guid>();

        var hasCart = await _cartRepository.HasActiveCartAsync(dto.UserId, cancellationToken);

        if (hasCart)
            return Result<Guid>.Failure("User already has an active cart.");

        var cart = new Cart(dto.UserId);

        await _cartRepository.AddAsync(cart, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(cart.Id);
    }

    #endregion

    #region Admin

    public async Task<Result<CartAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetWithItemsAsync(id, cancellationToken);

        if (cart is null)
            return Result<CartAdminDto>.Failure("Cart not found.");

        var dto = _mapper.Map<CartAdminDto>(cart);

        return Result<CartAdminDto>.Success(dto);
    }

    public async Task<PagedResult<CartAdminDto>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var totalCount = await _cartRepository.CountActiveAsync(cancellationToken);

        var carts = await _cartRepository.ListPagedWithIncludesAsync(
            predicate: c => !c.IsDeleted,
            orderBy: c => c.CreatedDate,
            ascending: false,
            skip: (pageNumber - 1) * pageSize,
            take: pageSize,
            cancellationToken: cancellationToken,
            c => c.User,
            c => c.Items);

        var dtos = _mapper.Map<List<CartAdminDto>>(carts);

        return PagedResult<CartAdminDto>.Success(dtos, pageNumber, pageSize, totalCount);
    }

    #endregion

    #region Customer

    public async Task<Result<CartCustomerDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return Result<CartCustomerDto>.Failure("UserId is required.");

        var cart = await _cartRepository.GetActiveCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            return Result<CartCustomerDto>.Failure("Active cart not found.");

        var dto = _mapper.Map<CartCustomerDto>(cart);

        return Result<CartCustomerDto>.Success(dto);
    }

    #endregion

    #region Cart Items

    public async Task<Result> AddItemAsync(
        Guid cartId,
        CartItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto is null)
            return Result.Failure("Request cannot be null.");

        var validation = await _addItemValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToResult();

        // 🔴 مهم: جلوگیری از اجرای همزمان روی DbContext
        var cart = await _cartRepository.GetWithItemsAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        var product = await _productRepository.GetByIdWithCategoryAsync(dto.ProductId, cancellationToken);

        if (product is null || product.IsDeleted || !product.IsActive)
            return Result.Failure("Product not available.");

        var stock = product.Stock?.Value ?? 0;

        if (stock <= 0)
            return Result.Failure("Product is out of stock.");

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == dto.ProductId);

        var totalQuantity = (existingItem?.Quantity.Value ?? 0) + dto.Quantity;

        if (totalQuantity > stock)
            return Result.Failure("Insufficient product stock.");

        try
        {
            cart.AddItem(
                product.Id,
                product.Title.Value,
                product.Price.Amount,
                dto.Quantity);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Cart item added successfully. CartId: {CartId}",
                cart.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while adding item to cart. CartId: {CartId}",
                cartId);

            return Result.Failure("Unexpected error occurred while adding item to cart.");
        }
    }

    public async Task<Result> UpdateItemAsync(
        Guid cartId,
        CartUpdateItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto is null)
            return Result.Failure("Request cannot be null.");

        var validation = await _updateItemValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToResult();

        var cart = await _cartRepository.GetWithItemsAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        var product = await _productRepository.GetByIdWithCategoryAsync(dto.ProductId, cancellationToken);

        if (product is null || product.IsDeleted || !product.IsActive)
            return Result.Failure("Product not available.");

        var stock = product.Stock?.Value ?? 0;

        if (dto.Quantity > stock)
            return Result.Failure("Insufficient product stock.");

        try
        {
            cart.ChangeItemQuantity(dto.ProductId, dto.Quantity);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }

    public async Task<Result> RemoveItemAsync(
        Guid cartId,
        Guid productId,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        try
        {
            cart.RemoveItem(productId, performedBy);

            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }

    public async Task<Result> ClearAsync(
        Guid cartId,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        cart.Clear(performedBy);

        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Soft Delete

    public async Task<Result> DeleteAsync(
        Guid cartId,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        await _cartRepository.SoftDeleteAsync(cartId, performedBy, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RestoreAsync(
        Guid cartId,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found.");

        await _cartRepository.RestoreAsync(cartId, performedBy, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion
}
