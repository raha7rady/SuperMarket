namespace SuperMarket.Application.DTOs.Reviews;

public sealed class ReviewUpdateDto
{
    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;
}
