using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.DTOs.Categories;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Admin.ViewModels.Categories;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class CategoriesController : Controller
{
    private const int DefaultPageSize = 10;

    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService
            ?? throw new ArgumentNullException(nameof(categoryService));
        _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
    }

    // ============================================================
    // LIST
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
            pageNumber = 1;

        if (pageSize <= 0)
            pageSize = DefaultPageSize;

        var result = await _categoryService.GetPagedForAdminAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;

            return View(new CategoryListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        var viewModel = new CategoryListViewModel
        {
            Items = _mapper.Map<IReadOnlyList<CategoryListItemViewModel>>(result.Value),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };

        return View(viewModel);
    }

    // ============================================================
    // CREATE
    // ============================================================

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CategoryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CategoryCreateViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<CategoryCreateDto>(model);
        var result = await _categoryService.CreateAsync(dto, cancellationToken);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.FirstError ?? "امکان ایجاد دسته‌بندی وجود ندارد.");
            return View(model);
        }

        TempData["Success"] = "دسته‌بندی با موفقیت ایجاد شد.";
        return RedirectToAction(nameof(Index));
    }

    // ============================================================
    // EDIT
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return NotFound();

        var result = await _categoryService.GetByIdForAdminAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;
            return RedirectToAction(nameof(Index));
        }

        var viewModel = _mapper.Map<CategoryEditViewModel>(result.Value);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        CategoryEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<CategoryUpdateDto>(model);
        var result = await _categoryService.UpdateAsync(dto, cancellationToken);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.FirstError ?? "امکان ویرایش دسته‌بندی وجود ندارد.");
            return View(model);
        }

        TempData["Success"] = "دسته‌بندی با موفقیت ویرایش شد.";
        return RedirectToAction(nameof(Index));
    }

    // ============================================================
    // DELETE
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return RedirectToAction(nameof(Index));

        var result = await _categoryService.DeleteAsync(id, cancellationToken);

        TempData[result.IsFailure ? "Error" : "Success"] =
            result.IsFailure ? result.FirstError : "دسته‌بندی با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}
