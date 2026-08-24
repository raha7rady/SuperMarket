namespace SuperMarket.Application.DTOs.Cart;

public sealed class CartItemDetailDto
{
    public Guid ProductId { get; set; }

    public string ProductTitle { get; set; } = string.Empty;

    public string? ProductImageUrl { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal => Price * Quantity;
}