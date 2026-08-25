using System;
using System.Threading;
using System.Threading.Tasks;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review, Guid>
    {
        Task<(double AverageRating, int ReviewCount)> GetRatingSummaryAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
