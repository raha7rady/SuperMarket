using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.ViewModels;

public sealed class RegisterViewModel
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "وارد کردن نام الزامی است.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "نام باید بین 2 تا 100 کاراکتر باشد.")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "نام خانوادگی")]
    [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "نام خانوادگی باید بین 2 تا 100 کاراکتر باشد.")]
    public string LastName { get; set; } = string.Empty;

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
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "رمز عبور باید حداقل 8 کاراکتر باشد.")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "تکرار رمز عبور")]
    [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "رمز عبور و تکرار آن یکسان نیست.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}