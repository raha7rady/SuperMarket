using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.Products.Queries;

public sealed class ProductCustomerQuery
{
    private const int MaxPageSize = 48;

    private int _pageNumber = 1;
    private int _pageSize = 12;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value <= 0 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize =
            value <= 0 ? 12 :
            value > MaxPageSize ? MaxPageSize :
            value;
    }

    public Guid? CategoryId { get; init; }

    public string? SearchTerm { get; init; }

    public bool OnlyInStock { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public ProductSortBy SortBy { get; init; } = ProductSortBy.DisplayOrder;

    // =========================
    // Validation helpers
    // =========================

    public bool HasValidPriceRange =>
        !MinPrice.HasValue ||
        !MaxPrice.HasValue ||
        MinPrice <= MaxPrice;

    public string? NormalizedSearchTerm =>
        string.IsNullOrWhiteSpace(SearchTerm)
            ? null
            : SearchTerm.Trim();
}