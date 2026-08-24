namespace SuperMarket.Application.DTOs.Users;

public sealed class UserResetPasswordDto
{
    public Guid Id { get; init; }

    public string NewPassword { get; init; } = string.Empty;

    public string ConfirmPassword { get; init; } = string.Empty;
}