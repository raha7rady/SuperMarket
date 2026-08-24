using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.ViewModels;

public sealed class LoginViewModel
{
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "وارد کردن ایمیل الزامی است.")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
    [DataType(DataType.EmailAddress)]
    [StringLength(
        256,
        ErrorMessage = "ایمیل نمی‌تواند بیشتر از 256 کاراکتر باشد.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "رمز عبور")]
    [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}