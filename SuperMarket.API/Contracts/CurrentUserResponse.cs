namespace SuperMarket.API.Contracts;

public sealed class CurrentUserResponse
{
    public Guid UserId { get; init; }

    public string? Email { get; init; }

    public string FullName { get; init; } = string.Empty;

    public bool IsAuthenticated { get; init; }
}
