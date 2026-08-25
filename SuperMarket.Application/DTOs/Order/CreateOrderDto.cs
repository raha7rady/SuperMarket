namespace SuperMarket.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public CheckoutDetailsDto? CheckoutDetails { get; set; }
    }
}
