using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.Areas.Customer.ViewModels.Cart
{
    public sealed class UpdateCartItemViewModel
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}