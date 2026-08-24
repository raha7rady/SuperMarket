namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductDetailsViewModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public string ImageUrl { get; init; } = string.Empty;

    public string CategoryName { get; init; } = string.Empty;

    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public bool IsInStock => Stock > 0;
}