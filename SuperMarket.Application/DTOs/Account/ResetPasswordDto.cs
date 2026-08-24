namespace SuperMarket.Application.DTOs.Account;

public sealed class ResetPasswordDto
{
    public string Email { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public string NewPassword { get; init; } = string.Empty;

    public string ConfirmPassword { get; init; } = string.Empty;
}