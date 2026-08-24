
namespace SuperMarket.Application.DTOs.Cart
{
    public class CartCustomerDto
    {
        public Guid Id { get; set; }
        public List<CartItemDetailDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
