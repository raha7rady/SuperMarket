using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.ViewComponents;

public sealed class ProductFilterViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public ProductFilterViewComponent(
        ICategoryService categoryService,
        IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IViewComponentResult> InvokeAsync(CustomerProductFilterViewModel model)
    {
        model ??= new CustomerProductFilterViewModel();

        var categoriesResult = await _categoryService.GetLookupAsync();

        model.Categories = categoriesResult.IsSuccess
            ? _mapper.Map<IReadOnlyList<CustomerCategoryItemViewModel>>(categoriesResult.Value)
            : Array.Empty<CustomerCategoryItemViewModel>();

        return View(model);
    }
}