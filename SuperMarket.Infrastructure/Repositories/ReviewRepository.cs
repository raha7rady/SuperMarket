using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories;

public sealed class ReviewRepository : Repository<Review, Guid>, IReviewRepository
{
    public ReviewRepository(SuperMarketDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<(double AverageRating, int ReviewCount)> GetRatingSummaryAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var query = ReadOnlyQuery()
            .Where(r => r.ProductId == productId && !r.IsDeleted);

        var count = await query.CountAsync(cancellationToken);

        if (count == 0)
        {
            return (0d, 0);
        }

        var average = await query.AverageAsync(r => (double)r.Rating, cancellationToken);

        return (average, count);
    }
}
