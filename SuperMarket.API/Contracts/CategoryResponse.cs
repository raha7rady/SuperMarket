namespace SuperMarket.API.Contracts;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string? ImageUrl { get; init; }

    public string? Description { get; init; }

    public string? Badge { get; init; }

    public int ItemCount { get; init; }
}
