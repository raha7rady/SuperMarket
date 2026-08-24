using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Route("products")]
public sealed class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IProductService productService,
        ICategoryService categoryService,
        IMapper mapper,
        ILogger<ProductController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] CustomerProductFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        filter ??= new CustomerProductFilterViewModel();
        filter.Normalize();

        if (!filter.IsValidPriceRange)
        {
            TempData["ErrorMessage"] =
                "حداقل قیمت نمی‌تواند بزرگ‌تر از حداکثر قیمت باشد.";

            return View(await CreateEmptyListAsync(filter, cancellationToken));
        }

        var paging = Paging.Normalize(filter.PageNumber, filter.PageSize);

        var query = new ProductCustomerQuery
        {
            CategoryId = filter.CategoryId,
            SearchTerm = filter.SearchTerm,
            OnlyInStock = filter.OnlyInStock,
            MinPrice = filter.MinPrice,
            MaxPrice = filter.MaxPrice,
            SortBy = filter.SortBy,
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize
        };

        var productResult =
            await _productService.GetPagedForCustomerAsync(query, cancellationToken);

        if (productResult.IsFailure)
        {
            _logger.LogWarning(
                "Product load failed: {Errors}",
                string.Join(" | ", productResult.Errors));

            TempData["ErrorMessage"] = "خطا در دریافت اطلاعات محصولات.";

            return View(await CreateEmptyListAsync(filter, cancellationToken));
        }

        var categoryResult =
            await _categoryService.GetLookupAsync(cancellationToken);

        var products = productResult.Value?.ToList()
                       ?? new List<ProductCustomerDto>();

        var viewModel = new CustomerProductListViewModel
        {
            Products = _mapper.Map<IReadOnlyList<CustomerProductListItemViewModel>>(products),

            Categories = categoryResult.IsSuccess
                ? _mapper.Map<IReadOnlyList<CategoryLookupViewModel>>(categoryResult.Value)
                : Array.Empty<CategoryLookupViewModel>(),

            Filters = filter,

            Pagination = new CustomerProductPaginationViewModel
            {
                PageNumber = Math.Max(1, productResult.PageNumber),
                PageSize = Math.Max(1, productResult.PageSize),
                TotalCount = Math.Max(0, productResult.TotalCount)
            }
        };

        return View(viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return RedirectToAction("NotFound", "Error", new { area = "" });

        var result = await _productService.GetByIdForCustomerAsync(id, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return RedirectToAction("NotFound", "Error", new { area = "" });

        var viewModel = _mapper.Map<CustomerProductDetailsViewModel>(result.Value);

        return View(viewModel);
    }

    private async Task<CustomerProductListViewModel> CreateEmptyListAsync(
        CustomerProductFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        var categoryResult =
            await _categoryService.GetLookupAsync(cancellationToken);

        return new CustomerProductListViewModel
        {
            Products = Array.Empty<CustomerProductListItemViewModel>(),

            Categories = categoryResult.IsSuccess
                ? _mapper.Map<IReadOnlyList<CategoryLookupViewModel>>(categoryResult.Value)
                : Array.Empty<CategoryLookupViewModel>(),

            Filters = filter,

            Pagination = new CustomerProductPaginationViewModel
            {
                PageNumber = 1,
                PageSize = filter?.PageSize > 0 ? filter.PageSize : 12,
                TotalCount = 0
            }
        };
    }
}