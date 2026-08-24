namespace SuperMarket.Application.DTOs.Products;

public sealed class ProductCustomerDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = null!;

    public decimal Price { get; init; }

    // TODO:
    // Replace with real discount system later.
    public decimal FinalPrice { get; init; }

    public bool HasDiscount { get; init; }

    public string ImageUrl { get; init; } = null!;

    public string CategoryName { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Slug { get; init; } = null!;

    public int Stock { get; init; }

    public bool IsInStock => Stock > 0;
}