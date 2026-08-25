using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.API.Mapping;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public sealed class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;
    private readonly ICurrentUserService _currentUser;

    public WishlistController(IWishlistService wishlistService, ICurrentUserService currentUser)
    {
        _wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<WishlistItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _wishlistService.GetByUserAsync(
            _currentUser.DomainUserId,
            pageNumber,
            pageSize,
            cancellationToken);

        var response = new PagedResponse<WishlistItemResponse>
        {
            Items = result.Value.Select(w => w.ToResponse()).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };

        return Ok(response);
    }

    [HttpPost("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _wishlistService.AddAsync(_currentUser.DomainUserId, productId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { id = result.Value });
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken cancellationToken)
    {
        await _wishlistService.RemoveAsync(_currentUser.DomainUserId, productId, cancellationToken);

        return NoContent();
    }
}
