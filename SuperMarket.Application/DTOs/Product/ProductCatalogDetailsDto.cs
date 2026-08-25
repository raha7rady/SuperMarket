namespace SuperMarket.Application.DTOs.Products;

public sealed class ProductCatalogDetailsDto
{
    public decimal? CompareAtPrice { get; init; }

    public string? Brand { get; init; }

    public string? Barcode { get; init; }

    public string? Unit { get; init; }

    public IReadOnlyList<string>? Tags { get; init; }

    public IReadOnlyList<string>? DietaryTags { get; init; }

    public IReadOnlyList<string>? GalleryImages { get; init; }

    public bool IsSpecialDeal { get; init; }

    public bool IsBestSeller { get; init; }

    public DateTimeOffset? DealEndTime { get; init; }
}
