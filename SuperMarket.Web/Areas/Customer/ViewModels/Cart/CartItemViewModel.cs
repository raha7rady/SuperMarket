namespace SuperMarket.Web.Areas.Customer.ViewModels.Cart
{
    public sealed class CartItemViewModel
    {
        public Guid ProductId { get; set; }

        public string ProductTitle { get; set; } = string.Empty;

        public string? ProductImageUrl { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}