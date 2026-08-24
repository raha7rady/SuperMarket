using SuperMarket.Domain.Enums;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;

namespace SuperMarket.Web.Areas.Customer.ViewModels.Products;

public sealed class CustomerProductFilterViewModel
{
    public Guid? CategoryId { get; set; }

    public string? CategorySlug { get; set; }

    public string? SearchTerm { get; set; }

    public bool OnlyInStock { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public ProductSortBy SortBy { get; set; } = ProductSortBy.DisplayOrder;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 12;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, 48);
    }

    // =========================
    // Validation helpers
    // =========================


    public IReadOnlyList<CustomerCategoryItemViewModel>
    Categories
    { get; set; }
    = Array.Empty<CustomerCategoryItemViewModel>();

    public bool HasSearch =>
        !string.IsNullOrWhiteSpace(SearchTerm);

    public bool HasPriceFilter =>
        MinPrice.HasValue || MaxPrice.HasValue;

    public bool HasCategory =>
        CategoryId.HasValue || !string.IsNullOrWhiteSpace(CategorySlug);

    public bool IsValidPriceRange =>
        !MinPrice.HasValue ||
        !MaxPrice.HasValue ||
        MinPrice <= MaxPrice;

    public bool HasFilters =>
        HasSearch || HasPriceFilter || HasCategory || OnlyInStock;

    public void Normalize()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue &&
            MinPrice > MaxPrice)
        {
            (MinPrice, MaxPrice) = (MaxPrice, MinPrice);
        }
    }
}