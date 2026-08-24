using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.ViewModels;

public sealed class ResendConfirmationEmailViewModel
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
}