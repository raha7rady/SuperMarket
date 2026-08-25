using System.ComponentModel.DataAnnotations;

namespace SuperMarket.API.Contracts;

public sealed class CreateReviewRequest
{
    [Required]
    public Guid ProductId { get; init; }

    [Range(1, 5)]
    public int Rating { get; init; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Comment { get; init; } = string.Empty;
}
