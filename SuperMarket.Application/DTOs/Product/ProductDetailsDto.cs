


namespace SuperMarket.Application.DTOs.Products;

public sealed class ProductDetailsDto
{
    public Guid Id { get; init; }

    public string Title { get; init; }
        = string.Empty;
    public string Slug { get; init; }
    = string.Empty;

    public string Description { get; init; }
        = string.Empty;

    public string ImageUrl { get; init; }
        = string.Empty;

    public string CategoryName { get; init; }
        = string.Empty;

    public decimal Price { get; init; }

    public decimal FinalPrice { get; init; }

    public bool HasDiscount { get; init; }

    public int Stock { get; init; }
}