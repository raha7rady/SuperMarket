namespace SuperMarket.API.Contracts;

public sealed class ProductSummaryResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? ImageUrl { get; init; }

    public decimal Price { get; init; }
}
