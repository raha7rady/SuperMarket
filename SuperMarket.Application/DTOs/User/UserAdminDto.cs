using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.DTOs.Users;

public sealed class UserAdminDto
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public UserRole Role { get; init; }
    public int OrderCount { get; init; }

    public int CartItemCount { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public bool IsDeleted { get; init; }
}