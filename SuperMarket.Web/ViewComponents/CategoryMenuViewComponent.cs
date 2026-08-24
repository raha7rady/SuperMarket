using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;

namespace SuperMarket.Web.ViewComponents;

public sealed class CategoryMenuViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryMenuViewComponent(
        ICategoryService categoryService,
        IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid? selectedCategoryId)
    {
        var result = await _categoryService.GetLookupAsync();

        if (!result.IsSuccess || result.Value is null)
        {
            return View(new List<CustomerCategoryItemViewModel>());
        }

        var categories = _mapper.Map<List<CustomerCategoryItemViewModel>>(result.Value);

        var viewModel = categories
            .Select(c => new CustomerCategoryItemViewModel
            {
                Id = c.Id,
                Title = c.Title,
                IsSelected = selectedCategoryId.HasValue && c.Id == selectedCategoryId.Value
            })
            .ToList();

        return View(viewModel);
    }
}