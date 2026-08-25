using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;
using SuperMarket.Web.ViewModels;
using SuperMarket.Web.ViewModels.Home;

namespace SuperMarket.Web.Controllers;

[AllowAnonymous]
public sealed class HomeController : Controller
{
    private const int FeaturedProductsCount = 8;

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IProductService productService,
        ICategoryService categoryService,
        IMapper mapper,
        ILogger<HomeController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categoryResult = await _categoryService.GetLookupAsync(cancellationToken);

        if (categoryResult.IsFailure)
        {
            _logger.LogWarning(
                "Home category load failed: {Errors}",
                string.Join(" | ", categoryResult.Errors));
        }

        var categories = categoryResult.IsSuccess && categoryResult.Value is not null
            ? _mapper.Map<IReadOnlyList<CustomerCategoryItemViewModel>>(categoryResult.Value)
            : Array.Empty<CustomerCategoryItemViewModel>();

        var featuredQuery = new ProductCustomerQuery
        {
            PageNumber = 1,
            PageSize = FeaturedProductsCount
        };

        var productResult =
            await _productService.GetPagedForCustomerAsync(featuredQuery, cancellationToken);

        if (productResult.IsFailure)
        {
            _logger.LogWarning(
                "Home featured product load failed: {Errors}",
                string.Join(" | ", productResult.Errors));
        }

        var featuredProducts = productResult.IsSuccess && productResult.Value is not null
            ? _mapper.Map<IReadOnlyList<CustomerProductCardViewModel>>(productResult.Value)
            : Array.Empty<CustomerProductCardViewModel>();

        var model = new HomeIndexViewModel
        {
            Categories = categories,
            FeaturedProducts = featuredProducts
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId =
                Activity.Current?.Id ??
                HttpContext.TraceIdentifier
        });
    }
}