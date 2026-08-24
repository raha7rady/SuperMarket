namespace SuperMarket.Application.DTOs.Orders
{
    public class OrderAdminDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public List<OrderItemDetailDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
