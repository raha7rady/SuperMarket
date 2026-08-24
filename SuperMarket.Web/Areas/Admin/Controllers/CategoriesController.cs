using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Categories;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class CategoriesController : ControllerBase
{
    private const int MaxPageSize = 100;
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService
            ?? throw new ArgumentNullException(nameof(categoryService));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CategoryCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtRoute(
            "GetCategoryByIdForAdmin",
            new { id = result.Value },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] CategoryUpdateDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { errors = new[] { "Route id and body id do not match." } });

        var result = await _categoryService.UpdateAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }

    [HttpGet("{id:guid}", Name = "GetCategoryByIdForAdmin")]
    [ProducesResponseType(typeof(CategoryAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdForAdmin(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdForAdminAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryAdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPagedForAdmin(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest(new { errors = new[] { "PageNumber and PageSize must be greater than zero." } });

        if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        var result = await _categoryService
            .GetPagedForAdminAsync(pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/active")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetActive(
        Guid id,
        [FromQuery] bool isActive,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService
            .SetActiveAsync(id, isActive, cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }

    private IActionResult HandleFailure(Result result)
    {
        var response = new
        {
            errors = result.Errors,
            errorCode = result.ErrorCode
        };

        return result.ErrorCode?.ToLowerInvariant() switch
        {
            "notfound" => NotFound(response),
            "conflict" => Conflict(response),
            _ => BadRequest(response)
        };
    }
}