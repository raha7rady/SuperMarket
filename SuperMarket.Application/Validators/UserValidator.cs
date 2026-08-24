using FluentValidation;
using SuperMarket.Application.DTOs.Users;
using System.Text.RegularExpressions;

namespace SuperMarket.Application.Validators
{
    internal static class UserValidationConstants
    {
        public const int MinNameLength = 2;
        public const int MaxNameLength = 100;

        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 64;

        public const int MaxEmailLength = 256;
    }

    internal static class UserValidationHelpers
    {
        private static readonly Regex UpperCaseRegex =
            new(@"[A-Z]", RegexOptions.Compiled);

        private static readonly Regex LowerCaseRegex =
            new(@"[a-z]", RegexOptions.Compiled);

        private static readonly Regex DigitRegex =
            new(@"\d", RegexOptions.Compiled);

        public static bool BeValidName(string? name)
            => !string.IsNullOrWhiteSpace(name?.Trim());

        public static bool BeStrongPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return UpperCaseRegex.IsMatch(password) &&
                   LowerCaseRegex.IsMatch(password) &&
                   DigitRegex.IsMatch(password);
        }
    }

    // ============================================================
    // Create User
    // ============================================================
    public sealed class UserValidator : AbstractValidator<UserCreateDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(UserValidationConstants.MinNameLength)
                .MaximumLength(UserValidationConstants.MaxNameLength)
                .Must(UserValidationHelpers.BeValidName)
                .WithMessage("First name cannot be whitespace only.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(UserValidationConstants.MinNameLength)
                .MaximumLength(UserValidationConstants.MaxNameLength)
                .Must(UserValidationHelpers.BeValidName)
                .WithMessage("Last name cannot be whitespace only.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(UserValidationConstants.MaxEmailLength)
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(UserValidationConstants.MinPasswordLength)
                .MaximumLength(UserValidationConstants.MaxPasswordLength)
                .Must(UserValidationHelpers.BeStrongPassword)
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter and one number.");
        }
    }

    // ============================================================
    // Update User
    // ============================================================
    public sealed class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(UserValidationConstants.MinNameLength)
                .MaximumLength(UserValidationConstants.MaxNameLength)
                .Must(UserValidationHelpers.BeValidName)
                .WithMessage("First name cannot be whitespace only.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(UserValidationConstants.MinNameLength)
                .MaximumLength(UserValidationConstants.MaxNameLength)
                .Must(UserValidationHelpers.BeValidName)
                .WithMessage("Last name cannot be whitespace only.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(UserValidationConstants.MaxEmailLength)
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }

    // ============================================================
    // Change Password
    // ============================================================
    public sealed class UserChangePasswordValidator : AbstractValidator<UserResetPasswordDto>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(UserValidationConstants.MinPasswordLength)
                .MaximumLength(UserValidationConstants.MaxPasswordLength)
                .Must(UserValidationHelpers.BeStrongPassword)
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter and one number.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match.");
        }
    }

    // ============================================================
    // Admin Role Update
    // ============================================================
    public sealed class UserAdminValidator : AbstractValidator<UserAdminDto>
    {
        public UserAdminValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid user role.");
        }
    }
}
