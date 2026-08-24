using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.ViewModels;

public sealed class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Display(Name = "رمز عبور جدید")]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(
        100,
        MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    [Display(Name = "تکرار رمز عبور")]
    [Required]
    [DataType(DataType.Password)]
    [Compare(
        nameof(NewPassword),
        ErrorMessage = "رمز عبور و تکرار آن یکسان نیست.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}