namespace SuperMarket.API.Contracts;

public sealed class CheckoutRequest
{
    public string RecipientFullName { get; init; } = string.Empty;

    public string RecipientPhone { get; init; } = string.Empty;

    public string Province { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string AddressLine { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string? Plaque { get; init; }

    public string? Unit { get; init; }

    public string? DeliveryNote { get; init; }

    public string DeliveryOption { get; init; } = string.Empty;

    public string PaymentMethod { get; init; } = string.Empty;

    public decimal ShippingCost { get; init; }

    public string? CouponCode { get; init; }

    public decimal CouponDiscount { get; init; }
}
