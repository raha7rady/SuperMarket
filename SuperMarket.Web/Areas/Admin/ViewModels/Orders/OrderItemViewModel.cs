namespace SuperMarket.Web.Areas.Admin.ViewModels.Orders;

public sealed class OrderItemViewModel
{
    public Guid ProductId { get; init; }

    public string ProductTitle { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Quantity { get; init; }

    public decimal SubTotal => Price * Quantity;
}