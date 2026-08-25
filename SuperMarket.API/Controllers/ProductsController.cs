using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.API.Mapping;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Domain.Enums;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductCustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? searchQuery,
        [FromQuery] bool onlyInStock,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var query = new ProductCustomerQuery
        {
            CategoryId = categoryId,
            SearchTerm = searchQuery,
            OnlyInStock = onlyInStock,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = MapSortBy(sortBy),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        if (!query.HasValidPriceRange)
        {
            return Problem(
                detail: "minPrice cannot be greater than maxPrice.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _productService.GetPagedForCustomerAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(
                detail: string.Join(" ", result.Errors),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(result.ToPagedResponse());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductCustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdForCustomerAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Value);
    }

    private static ProductSortBy MapSortBy(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "newest" => ProductSortBy.Newest,
            "pricelow" => ProductSortBy.PriceLowToHigh,
            "pricehigh" => ProductSortBy.PriceHighToLow,
            "name" => ProductSortBy.NameAscending,
            _ => ProductSortBy.DisplayOrder
        };
    }
}
