using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Web.ViewModels;

public sealed class ForgotPasswordViewModel
{
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "وارد کردن ایمیل الزامی است.")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
    [DataType(DataType.EmailAddress)]
    [StringLength(
        256,
        ErrorMessage = "ایمیل نمی‌تواند بیشتر از 256 کاراکتر باشد.")]
    public string Email { get; set; } = string.Empty;
}