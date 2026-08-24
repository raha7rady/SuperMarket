using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.DTOs.Account;

public sealed class LoginResultDto
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public UserRole Role { get; init; }

    public bool RequiresTwoFactor { get; init; }

    public bool IsLockedOut { get; init; }

    public bool RequiresEmailConfirmation { get; init; }

    public bool IsAdmin =>
        Role == UserRole.Admin ||
        Role == UserRole.SuperAdmin ||
        Role == UserRole.Staff;
}