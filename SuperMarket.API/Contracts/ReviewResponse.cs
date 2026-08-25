namespace SuperMarket.API.Contracts;

public sealed class ReviewResponse
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public Guid UserId { get; init; }

    public string ReviewerName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}
