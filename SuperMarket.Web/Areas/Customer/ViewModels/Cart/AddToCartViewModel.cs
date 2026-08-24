using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.Areas.Customer.ViewModels.Cart
{
    public sealed class AddToCartViewModel
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }
}