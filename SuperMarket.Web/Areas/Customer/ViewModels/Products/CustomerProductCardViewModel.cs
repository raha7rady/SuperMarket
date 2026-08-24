namespace SuperMarket.Web.Areas.Customer.ViewModels.Products;

public class CustomerProductCardViewModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public decimal FinalPrice { get; init; }

    public bool HasDiscount { get; init; }

    public int Stock { get; init; }

    public bool IsInStock => Stock > 0;

    public string? ShortDescription { get; init; }

    // =====================================
    // UI Helpers (SAFE & OPTIMIZED)
    // =====================================

    public bool HasValidDiscount =>
        HasDiscount && Price > 0 && FinalPrice < Price;

    public decimal DiscountPercentValue =>
        HasValidDiscount
            ? (Price - FinalPrice) / Price * 100m
            : 0;

    public int DiscountPercent =>
        HasValidDiscount
            ? (int)Math.Round(DiscountPercentValue)
            : 0;

    public bool ShowOldPrice =>
        HasValidDiscount;

    public string PriceText =>
        FinalPrice.ToString("N0");

    public string OldPriceText =>
        Price.ToString("N0");

    public string StockStatusText =>
        Stock switch
        {
            <= 0 => "ناموجود",
            <= 5 => $"فقط {Stock} عدد باقی مانده",
            _ => "موجود"
        };
}