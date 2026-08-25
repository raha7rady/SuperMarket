namespace SuperMarket.API.Contracts;

public sealed class ReviewSummaryResponse
{
    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }
}
