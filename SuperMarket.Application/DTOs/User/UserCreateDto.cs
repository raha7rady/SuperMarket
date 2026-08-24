using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.DTOs.Users;

public sealed class UserCreateDto
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public UserRole Role { get; init; } = UserRole.Customer;
}