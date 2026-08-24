namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductListItemViewModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public bool IsInStock => Stock > 0;
}