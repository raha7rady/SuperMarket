using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.API.Mapping;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Reviews;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUser;

    public ReviewsController(IReviewService reviewService, ICurrentUserService currentUser)
    {
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProduct(
        [FromQuery] Guid productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.GetByProductAsync(productId, pageNumber, pageSize, cancellationToken);

        var response = new PagedResponse<ReviewResponse>
        {
            Items = result.Value.Select(r => r.ToResponse()).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };

        return Ok(response);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ReviewSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary([FromQuery] Guid productId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetSummaryAsync(productId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new ReviewSummaryResponse
        {
            AverageRating = result.Value.AverageRating,
            ReviewCount = result.Value.ReviewCount
        });
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await _reviewService.CreateAsync(
            _currentUser.DomainUserId,
            new ReviewCreateDto
            {
                ProductId = request.ProductId,
                Rating = request.Rating,
                Comment = request.Comment
            },
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return StatusCode(StatusCodes.Status201Created, new { id = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await _reviewService.UpdateAsync(
            id,
            _currentUser.DomainUserId,
            new ReviewUpdateDto
            {
                Rating = request.Rating,
                Comment = request.Comment
            },
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Errors.Contains("Review not found.")
                ? NotFound(new { errors = result.Errors })
                : BadRequest(new { errors = result.Errors });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reviewService.DeleteAsync(id, _currentUser.DomainUserId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { errors = result.Errors });
        }

        return NoContent();
    }
}
