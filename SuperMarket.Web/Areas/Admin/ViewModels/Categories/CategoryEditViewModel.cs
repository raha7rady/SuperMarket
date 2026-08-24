using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.Areas.Admin.ViewModels.Categories;

public sealed class CategoryEditViewModel : CategoryFormViewModel
{
    [Required]
    public Guid Id { get; set; }

    public bool IsActive { get; set; }

    // برای آینده (اگر Concurrency اضافه شد)
    public byte[]? RowVersion { get; set; }
}