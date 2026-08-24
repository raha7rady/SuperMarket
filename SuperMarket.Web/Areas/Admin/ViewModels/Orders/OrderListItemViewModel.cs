namespace SuperMarket.Web.Areas.Admin.ViewModels.Orders;

public sealed class OrderListItemViewModel
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public OrderStatusViewModel OrderStatus { get; init; }

    public PaymentStatusViewModel PaymentStatus { get; init; }

    public decimal TotalPrice { get; init; }

    public int ItemsCount { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public bool IsPaid =>
        PaymentStatus == PaymentStatusViewModel.Paid;

    public bool IsCompleted =>
        OrderStatus == OrderStatusViewModel.Delivered;

    public bool CanCancel =>
        OrderStatus != OrderStatusViewModel.Delivered &&
        OrderStatus != OrderStatusViewModel.Cancelled;
}