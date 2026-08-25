namespace SuperMarket.Application.DTOs.Orders
{
    public class OrderCustomerDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public List<OrderItemDetailDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public OrderRecipientDto? Recipient { get; set; }
        public string? DeliveryOption { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public string? CouponCode { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal FinalPayable { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
