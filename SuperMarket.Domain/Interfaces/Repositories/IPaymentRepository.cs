using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payment, Guid>
    {
        Task<Payment?> GetByIdWithOrderAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Payment>> GetByOrderIdAsync(
            Guid orderId,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default);

        Task<int> CountByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task SoftDeleteAsync(
            Guid paymentId,
            Guid deletedBy,
            CancellationToken cancellationToken = default);
    }
}
