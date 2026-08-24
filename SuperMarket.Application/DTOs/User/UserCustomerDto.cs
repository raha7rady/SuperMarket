namespace SuperMarket.Application.DTOs.Users;

public sealed class UserCustomerDto
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    //public string Role { get; init; } = string.Empty;
}