namespace SuperMarket.Web.Areas.Admin.ViewModels.Users
{
    public sealed class UserChangePasswordViewModel
    {
        public Guid Id { get; init; }

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}