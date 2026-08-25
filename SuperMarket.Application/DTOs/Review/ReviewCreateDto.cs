namespace SuperMarket.Application.DTOs.Reviews;

public sealed class ReviewCreateDto
{
    public Guid ProductId { get; init; }

    public int Rating { get; init; }

    public string Comment { get; init; } = string.Empty;
}
