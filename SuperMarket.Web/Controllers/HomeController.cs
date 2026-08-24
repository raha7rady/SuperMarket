using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;
using SuperMarket.Web.ViewModels;
using SuperMarket.Web.ViewModels.Home;

namespace SuperMarket.Web.Controllers;

[AllowAnonymous]
public sealed class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new HomeIndexViewModel
        {
            Categories =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "نوشیدنی ها"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "لبنیات"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "تنقلات"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "مواد پروتئینی"
                }
            ],

            FeaturedProducts =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "شیر کم چرب",
                    Slug = "milk",
                    CategoryName = "لبنیات",
                    Price = 120000,
                    FinalPrice = 99000,
                    HasDiscount = true,
                    Stock = 15,
                    ImageUrl = "/images/products/default-product.jpg"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "چیپس ساده",
                    Slug = "chips",
                    CategoryName = "تنقلات",
                    Price = 45000,
                    FinalPrice = 45000,
                    HasDiscount = false,
                    Stock = 8,
                    ImageUrl = "/images/products/default-product.jpg"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "آب معدنی",
                    Slug = "water",
                    CategoryName = "نوشیدنی",
                    Price = 15000,
                    FinalPrice = 12000,
                    HasDiscount = true,
                    Stock = 50,
                    ImageUrl = "/images/products/default-product.jpg"
                }
            ]
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