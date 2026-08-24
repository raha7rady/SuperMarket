using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductCreateViewModel
{
    [Required]
    [MaxLength(200)]
    [Display(Name = "Product Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0.01", "999999999")]
    [Display(Name = "Price")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    [Display(Name = "Stock Quantity")]
    public int Stock { get; set; }

    [Required]
    [Url]
    [Display(Name = "Image URL")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }

    [Range(0, 9999)]
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; }
        = Enumerable.Empty<SelectListItem>();
}