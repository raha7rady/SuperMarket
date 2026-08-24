
namespace SuperMarket.Application.DTOs.Cart
{
    public class CartUpdateItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
