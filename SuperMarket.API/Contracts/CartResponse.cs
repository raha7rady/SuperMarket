namespace SuperMarket.API.Contracts;

public sealed class CartResponse
{
    public Guid Id { get; init; }

    public IReadOnlyList<CartItemResponse> Items { get; init; } = Array.Empty<CartItemResponse>();

    public int TotalItems { get; init; }

    public decimal TotalAmount { get; init; }
}
