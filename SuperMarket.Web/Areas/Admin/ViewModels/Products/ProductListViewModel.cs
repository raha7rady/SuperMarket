namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductListViewModel
{
    public IReadOnlyList<ProductListItemViewModel> Items { get; init; }
        = Array.Empty<ProductListItemViewModel>();

    public ProductFilterViewModel Filter { get; init; }
        = new();

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }

    public bool HasPreviousPage => Filter.PageNumber > 1;

    public bool HasNextPage => Filter.PageNumber < TotalPages;
}