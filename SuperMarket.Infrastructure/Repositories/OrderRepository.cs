
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, Guid>, IOrderRepository
    {
        public OrderRepository(SuperMarketDbContext dbContext) : base(dbContext)
        {
        }

        public override IQueryable<Order> Query(params Expression<Func<Order, object>>[] includes)
        {
            IQueryable<Order> query = _dbSet.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return query;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(Guid userId, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && o.UserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdWithItemsAsync(Guid userId, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && o.OrderStatus == status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByPaymentStatusAsync(PaymentStatus status, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && o.PaymentStatus == status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId, cancellationToken);
        }

        public async Task<Order?> GetFullGraphAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> ListActiveAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> ListPagedWithIncludesAsync<TOrderKey>(
            Expression<Func<Order, bool>>? predicate = null,
            Expression<Func<Order, TOrderKey>>? orderBy = null,
            bool ascending = true,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default,
            params Expression<Func<Order, object>>[] includes)
        {
            IQueryable<Order> query = _dbSet.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            if (orderBy != null)
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            else
                query = query.OrderByDescending(o => o.CreatedDate);

            if (skip > 0)
                query = query.Skip(skip);
            if (take > 0)
                query = query.Take(take);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> CountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(o => !o.IsDeleted && o.UserId == userId, cancellationToken);
        }

        public async Task<int> CountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(o => !o.IsDeleted && o.OrderStatus == status, cancellationToken);
        }

        public async Task SoftDeleteAsync(Guid orderId, Guid deletedBy, CancellationToken cancellationToken = default)
        {
            var order = await GetByIdAsync(orderId, cancellationToken);
            if (order == null || order.IsDeleted) return;

            order.SoftDelete(deletedBy);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task RestoreAsync(Guid orderId, Guid restoredBy, CancellationToken cancellationToken = default)
        {
            var order = await GetByIdAsync(orderId, cancellationToken);
            if (order == null || !order.IsDeleted) return;

            order.Restore(restoredBy);
            await SaveChangesAsync(cancellationToken);
        }
    }
}
