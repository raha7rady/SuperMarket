namespace SuperMarket.Application.DTOs.Products;

public sealed class ProductCustomerDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = null!;

    public decimal Price { get; init; }

    public decimal FinalPrice { get; init; }

    public decimal? CompareAtPrice { get; init; }

    public bool HasValidDiscount { get; init; }

    /// <summary>Kept for backward compatibility with existing MVC view models.</summary>
    public bool HasDiscount => HasValidDiscount;

    public int? DiscountPercent { get; init; }

    public string ImageUrl { get; init; } = null!;

    public Guid CategoryId { get; init; }

    public string CategoryName { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Slug { get; init; } = null!;

    public int Stock { get; init; }

    public bool IsInStock => Stock > 0;

    public string? Brand { get; init; }

    public string? Barcode { get; init; }

    public string? Unit { get; init; }

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> DietaryTags { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> GalleryImages { get; init; } = Array.Empty<string>();

    public bool IsSpecialDeal { get; init; }

    public bool IsBestSeller { get; init; }

    public DateTimeOffset? DealEndTime { get; init; }
}
