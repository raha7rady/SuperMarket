namespace SuperMarket.Web.Areas.Customer.ViewModels.Cart
{
    public sealed class CartViewModel
    {
        public Guid CartId { get; set; }

        public IReadOnlyList<CartItemViewModel> Items { get; set; }
            = Array.Empty<CartItemViewModel>();

        public int TotalItems { get; set; }

        public decimal TotalAmount { get; set; }

        public bool HasItems => Items.Count > 0;
    }
}