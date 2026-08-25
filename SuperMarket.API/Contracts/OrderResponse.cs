namespace SuperMarket.API.Contracts;

public sealed class OrderResponse
{
    public Guid Id { get; init; }

    public string OrderNumber { get; init; } = string.Empty;

    public string OrderStatus { get; init; } = string.Empty;

    public string PaymentStatus { get; init; } = string.Empty;

    public IReadOnlyList<OrderItemResponse> Items { get; init; } = Array.Empty<OrderItemResponse>();

    public decimal TotalPrice { get; init; }

    public OrderRecipientResponse? Recipient { get; init; }

    public string? DeliveryOption { get; init; }

    public string? PaymentMethod { get; init; }

    public decimal ShippingCost { get; init; }

    public string? CouponCode { get; init; }

    public decimal CouponDiscount { get; init; }

    public decimal FinalPayable { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
