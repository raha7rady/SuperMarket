namespace SuperMarket.API.Contracts;

public sealed class WishlistItemResponse
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public string ProductTitle { get; init; } = string.Empty;

    public string? ProductImageUrl { get; init; }

    public decimal Price { get; init; }

    public bool IsInStock { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
