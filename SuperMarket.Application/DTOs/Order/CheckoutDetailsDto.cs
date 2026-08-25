using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.DTOs.Orders;

public sealed class CheckoutDetailsDto
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

    public DeliveryOption DeliveryOption { get; init; }

    public OrderPaymentMethod PaymentMethod { get; init; }

    public decimal ShippingCost { get; init; }

    public string? CouponCode { get; init; }

    public decimal CouponDiscount { get; init; }
}
