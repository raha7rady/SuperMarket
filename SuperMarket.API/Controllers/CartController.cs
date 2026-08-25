using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.API.Mapping;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Cart;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICurrentUserService _currentUser;

    public CartController(ICartService cartService, ICurrentUserService currentUser)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await GetOrCreateCartAsync(cancellationToken);

        if (result.IsFailure)
        {
            return Problem(
                detail: string.Join(" ", result.Errors),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(result.Value.ToResponse());
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] CartItemDto dto, CancellationToken cancellationToken)
    {
        var cartResult = await GetOrCreateCartAsync(cancellationToken);

        if (cartResult.IsFailure)
        {
            return Problem(
                detail: string.Join(" ", cartResult.Errors),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var addResult = await _cartService.AddItemAsync(cartResult.Value.Id, dto, cancellationToken);

        if (addResult.IsFailure)
        {
            return BadRequest(new { errors = addResult.Errors });
        }

        var refreshed = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        return Ok(refreshed.Value.ToResponse());
    }

    [HttpPut("items/{productId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(
        Guid productId,
        [FromBody] CartItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var cartResult = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        if (cartResult.IsFailure)
        {
            return NotFound(new { errors = cartResult.Errors });
        }

        var updateResult = await _cartService.UpdateItemAsync(
            cartResult.Value.Id,
            new CartUpdateItemDto { ProductId = productId, Quantity = request.Quantity },
            cancellationToken);

        if (updateResult.IsFailure)
        {
            return BadRequest(new { errors = updateResult.Errors });
        }

        var refreshed = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        return Ok(refreshed.Value.ToResponse());
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        var cartResult = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        if (cartResult.IsFailure)
        {
            return NotFound(new { errors = cartResult.Errors });
        }

        var removeResult = await _cartService.RemoveItemAsync(
            cartResult.Value.Id,
            productId,
            _currentUser.DomainUserId,
            cancellationToken);

        if (removeResult.IsFailure)
        {
            return BadRequest(new { errors = removeResult.Errors });
        }

        var refreshed = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        return Ok(refreshed.Value.ToResponse());
    }

    private async Task<Result<CartCustomerDto>> GetOrCreateCartAsync(CancellationToken cancellationToken)
    {
        var existing = await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);

        if (existing.IsSuccess)
        {
            return existing;
        }

        var createResult = await _cartService.CreateAsync(
            new CartCreateDto { UserId = _currentUser.DomainUserId },
            cancellationToken);

        if (createResult.IsFailure)
        {
            return Result<CartCustomerDto>.Failure(createResult.Errors);
        }

        return await _cartService.GetByUserIdAsync(_currentUser.DomainUserId, cancellationToken);
    }
}
