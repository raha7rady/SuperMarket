using SuperMarket.Web.Areas.Customer.ViewModels.Categories;

namespace SuperMarket.Web.Areas.Customer.ViewModels.Products;

public sealed class CustomerProductListViewModel
{
    public IReadOnlyList<CustomerProductListItemViewModel>
        Products
    { get; init; }
        = Array.Empty<CustomerProductListItemViewModel>();

    public CustomerProductFilterViewModel Filters
    { get; init; }
        = new();

    public CustomerProductPaginationViewModel Pagination
    { get; init; }
        = new();

    public IReadOnlyList<CategoryLookupViewModel> Categories
    {
        get;
        init;
    }
= Array.Empty<CategoryLookupViewModel>();

    public bool HasProducts =>
        Products.Count > 0;

    public bool IsEmpty =>
        !HasProducts;
}