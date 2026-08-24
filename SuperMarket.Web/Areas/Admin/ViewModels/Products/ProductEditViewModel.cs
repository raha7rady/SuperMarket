using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductEditViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "Product Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0.01", "999999999")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    [Url]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    [Range(0, 9999)]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; }
        = Enumerable.Empty<SelectListItem>();
}