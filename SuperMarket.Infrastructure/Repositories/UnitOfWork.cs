

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SuperMarketDbContext _dbContext;


    private IUserRepository? _userRepository;
    private IProductRepository? _productRepository;
    private ICategoryRepository? _categoryRepository;
    private ICartRepository? _cartRepository;
    private IOrderRepository? _orderRepository;
    private IPaymentRepository? _paymentRepository;


    public UnitOfWork(
        SuperMarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }



    public IUserRepository Users =>
        _userRepository ??=
            new UserRepository(_dbContext);



    public IProductRepository Products =>
        _productRepository ??=
            new ProductRepository(_dbContext);



    public ICategoryRepository Categories =>
        _categoryRepository ??=
            new CategoryRepository(_dbContext);



    public ICartRepository Carts =>
        _cartRepository ??=
            new CartRepository(_dbContext);



    public IOrderRepository Orders =>
        _orderRepository ??=
            new OrderRepository(_dbContext);



    public IPaymentRepository Payments =>
        _paymentRepository ??=
            new PaymentRepository(_dbContext);


    public IRepository<TEntity, Guid> Repository<TEntity>()
        where TEntity : class
    {
        return new Repository<TEntity, Guid>(_dbContext);
    }




    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(
            cancellationToken);
    }





    public async Task ExecuteTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy =
            _dbContext.Database.CreateExecutionStrategy();


        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    cancellationToken);


            try
            {
                await operation();


                await _dbContext.SaveChangesAsync(
                    cancellationToken);


                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        });
    }
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}