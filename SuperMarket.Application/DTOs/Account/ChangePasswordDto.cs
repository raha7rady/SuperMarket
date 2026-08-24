namespace SuperMarket.Application.DTOs.Account;

public sealed class ChangePasswordDto
{
    //public Guid UserId { get; init; }

    public string CurrentPassword { get; init; } = string.Empty;

    public string NewPassword { get; init; } = string.Empty;

    public string ConfirmPassword { get; init; } = string.Empty;
}