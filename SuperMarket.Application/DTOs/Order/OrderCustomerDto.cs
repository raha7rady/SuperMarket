namespace SuperMarket.Application.DTOs.Orders
{
    public class OrderCustomerDto
    {
        public Guid Id { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public List<OrderItemDetailDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
