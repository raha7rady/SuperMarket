
namespace SuperMarket.Application.DTOs.Orders
{
    public class OrderItemDetailDto
    {
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}
