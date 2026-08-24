using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment, Guid>, IPaymentRepository
    {
        public PaymentRepository(SuperMarketDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Payment?> GetByIdWithOrderAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByOrderIdAsync(
            Guid orderId,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted && p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(p => !p.IsDeleted && p.OrderId == orderId, cancellationToken);
        }

        public async Task SoftDeleteAsync(Guid paymentId, Guid deletedBy, CancellationToken cancellationToken = default)
        {
            var payment = await GetByIdAsync(paymentId, cancellationToken);
            if (payment == null || payment.IsDeleted) return;

            payment.SoftDelete(deletedBy);
            await SaveChangesAsync(cancellationToken);
        }
    }
}
