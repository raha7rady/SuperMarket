using System;
using System.Threading;
using System.Threading.Tasks;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity, Guid> Repository<TEntity>() where TEntity : class;

        IUserRepository Users { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task ExecuteTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
    }
}
