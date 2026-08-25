namespace SuperMarket.API.Contracts;

public sealed class OrderItemResponse
{
    public ProductSummaryResponse Product { get; init; } = null!;

    public int Quantity { get; init; }

    public decimal SubTotal { get; init; }
}
