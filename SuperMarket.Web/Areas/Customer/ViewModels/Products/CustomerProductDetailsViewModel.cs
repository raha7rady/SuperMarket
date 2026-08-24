namespace SuperMarket.Web.Areas.Customer.ViewModels.Products;

public sealed class CustomerProductDetailsViewModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public decimal FinalPrice { get; init; }

    public bool HasDiscount { get; init; }

    public int Stock { get; init; }

    public bool IsInStock => Stock > 0;

    public bool HasValidDiscount =>
        HasDiscount && Price > 0 && FinalPrice < Price;

    public bool ShowOldPrice =>
        HasValidDiscount;

    public int DiscountPercent =>
        HasValidDiscount
            ? (int)Math.Round((Price - FinalPrice) / Price * 100m)
            : 0;

    public string PriceText =>
        FinalPrice.ToString("N0");

    public string OldPriceText =>
        Price.ToString("N0");

    public string StockStatusText =>
        IsInStock ? "موجود در انبار" : "ناموجود";

    // =====================================
    // SEO (SAFE VERSION)
    // =====================================

    public string SeoTitle =>
        $"{Title} | فروشگاه سوپرمارکت";

    public string SeoDescription =>
        Truncate(Description, 160);

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        if (value.Length <= maxLength)
            return value;

        var cut = value[..maxLength];

        var lastSpace = cut.LastIndexOf(' ');

        return lastSpace > 40
            ? cut[..lastSpace]
            : cut;
    }
}