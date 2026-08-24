namespace SuperMarket.Application.DTOs.Account;

public sealed class LoginDto
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public bool RememberMe { get; init; }

}