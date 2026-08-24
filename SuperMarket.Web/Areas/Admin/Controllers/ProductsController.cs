using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Products.Queries;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Admin.ViewModels.Products;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IMapper mapper)
    {
        _productService = productService;
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(
        ProductFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

        var result = await _productService.GetPagedForAdminAsync(
            filter.PageNumber,
            filter.PageSize,
            cancellationToken);

        if (result.IsFailure)
        {
            AddErrors(result.Errors);
            return View(new ProductListViewModel { Filter = filter });
        }

        var vm = new ProductListViewModel
        {
            Items = _mapper.Map<IReadOnlyList<ProductListItemViewModel>>(result.Value),
            Filter = filter,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return NotFound();

        var result = await _productService.GetByIdForAdminAsync(id, cancellationToken);

        if (result.IsFailure)
            return NotFound();

        return View(_mapper.Map<ProductDetailsViewModel>(result.Value));
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return View(new ProductCreateViewModel
        {
            Categories = await GetCategoriesAsync(cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ProductCreateViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesAsync(cancellationToken);
            return View(model);
        }

        var result = await _productService.CreateAsync(
            _mapper.Map<ProductCreateDto>(model),
            cancellationToken);

        if (result.IsFailure)
        {
            AddErrors(result.Errors);
            model.Categories = await GetCategoriesAsync(cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return NotFound();

        var result = await _productService.GetByIdForAdminAsync(id, cancellationToken);

        if (result.IsFailure)
            return NotFound();

        var vm = _mapper.Map<ProductEditViewModel>(result.Value);
        vm.Categories = await GetCategoriesAsync(cancellationToken);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ProductEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesAsync(cancellationToken);
            return View(model);
        }

        var result = await _productService.UpdateAsync(
            _mapper.Map<ProductUpdateDto>(model),
            cancellationToken);

        if (result.IsFailure)
        {
            AddErrors(result.Errors);
            model.Categories = await GetCategoriesAsync(cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return RedirectToAction(nameof(Index));

        var result = await _productService.DeleteAsync(id, cancellationToken);

        TempData[result.IsFailure ? "Error" : "Success"] =
            result.IsFailure
                ? string.Join(" | ", result.Errors)
                : "Product deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return RedirectToAction(nameof(Index));

        var result = await _productService.SetActiveAsync(id, isActive, cancellationToken);

        if (result.IsFailure)
            TempData["Error"] = string.Join(" | ", result.Errors);

        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetCategoriesAsync(
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetLookupAsync(cancellationToken);

        if (result.IsFailure)
            return Enumerable.Empty<SelectListItem>();

        return result.Value.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        });
    }

    private void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);
    }
}