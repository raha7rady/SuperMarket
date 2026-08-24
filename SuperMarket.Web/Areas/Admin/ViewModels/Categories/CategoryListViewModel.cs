namespace SuperMarket.Web.Areas.Admin.ViewModels.Categories;

public sealed class CategoryListViewModel
{
    public IReadOnlyList<CategoryListItemViewModel> Items { get; init; }
        = Array.Empty<CategoryListItemViewModel>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages =>
        PageSize == 0 ? 0 :
        (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage =>
        PageNumber > 1;

    public bool HasNextPage =>
        PageNumber < TotalPages;

    public string? SearchTerm { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }
}