namespace SuperMarket.Application.DTOs.Account;

public sealed class RegisterResultDto
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public bool RequiresEmailConfirmation { get; init; }
}