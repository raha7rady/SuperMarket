namespace SuperMarket.Web.Areas.Admin.ViewModels.Orders;

public sealed class OrderDetailsViewModel
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public OrderStatusViewModel OrderStatus { get; init; }

    public PaymentStatusViewModel PaymentStatus { get; init; }

    public decimal TotalPrice { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    private IReadOnlyList<OrderItemViewModel> _items
        = Array.Empty<OrderItemViewModel>();

    public IReadOnlyList<OrderItemViewModel> Items
    {
        get => _items;
        init => _items = value ?? Array.Empty<OrderItemViewModel>();
    }

    public bool IsPaid =>
        PaymentStatus == PaymentStatusViewModel.Paid;

    public bool IsShipped =>
        OrderStatus == OrderStatusViewModel.Shipped;

    public bool IsDelivered =>
        OrderStatus == OrderStatusViewModel.Delivered;

    public bool IsCancelled =>
        OrderStatus == OrderStatusViewModel.Cancelled;

    public bool CanMarkAsPaid =>
        PaymentStatus != PaymentStatusViewModel.Paid;

    public bool CanMarkAsShipped =>
        OrderStatus == OrderStatusViewModel.Paid;

    public bool CanMarkAsDelivered =>
        OrderStatus == OrderStatusViewModel.Shipped;

    public bool CanCancel =>
        OrderStatus != OrderStatusViewModel.Delivered &&
        OrderStatus != OrderStatusViewModel.Cancelled;
}