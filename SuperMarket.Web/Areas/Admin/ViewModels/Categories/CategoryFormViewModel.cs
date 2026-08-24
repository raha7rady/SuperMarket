using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.Areas.Admin.ViewModels.Categories;

public abstract class CategoryFormViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000.")]
    public int DisplayOrder { get; set; }

    public string TrimmedTitle => Title?.Trim() ?? string.Empty;
}