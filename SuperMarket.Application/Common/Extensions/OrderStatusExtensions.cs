using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.Common.Extensions;

public static class OrderStatusExtensions
{
    public static string ToFrontendString(this OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "preparing",
            OrderStatus.Processing => "preparing",
            OrderStatus.Shipped => "dispatched",
            OrderStatus.Delivered => "delivered",
            OrderStatus.Canceled => "cancelled",
            OrderStatus.Returned => "cancelled",
            _ => "preparing"
        };
    }

    public static string ToFrontendString(this PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Pending => "pending",
            PaymentStatus.Processing => "pending",
            PaymentStatus.Paid => "paid",
            PaymentStatus.Failed => "failed",
            PaymentStatus.Refunded => "cancelled",
            PaymentStatus.Canceled => "cancelled",
            _ => "pending"
        };
    }

    public static string ToFriendlyOrderNumber(this Guid orderId)
    {
        return "SMP-" + orderId.ToString("N")[..8].ToUpperInvariant();
    }
}
