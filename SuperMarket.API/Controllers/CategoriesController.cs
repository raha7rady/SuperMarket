using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private const int MaxCategoriesPageSize = 200;

    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetPagedForCustomerAsync(1, MaxCategoriesPageSize, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(
                detail: string.Join(" ", result.Errors),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var response = result.Value.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Title = c.Title,
            Slug = c.Slug,
            ImageUrl = c.ImageUrl,
            Description = c.Description,
            Badge = c.Badge,
            ItemCount = c.ItemCount
        }).ToList();

        return Ok(response);
    }
}
