namespace SuperMarket.Application.DTOs.Products;

public sealed class ProductUpdateDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public string Description { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}
