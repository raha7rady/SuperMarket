
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories
{
    public class CategoryRepository
        : Repository<Category, Guid>,
          ICategoryRepository
    {
        public CategoryRepository(
            SuperMarketDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<Category> QueryAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<Category?> GetWithProductsAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(
                    c => c.Id == categoryId &&
                         !c.IsDeleted,
                    cancellationToken);
        }

        public async Task<Category?> GetByTitleAsync(
            string title,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.Title == title &&
                         !c.IsDeleted,
                    cancellationToken);
        }

        public async Task<Category?> GetByTitleAsync(
            string title,
            Guid? excludeId,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c =>
                    c.Title == title &&
                    !c.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    c => c.Id != excludeId.Value);
            }

            return await query
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> ListActiveAsync(
            Expression<Func<Category, bool>>? predicate = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<Category, object>>[] includes)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> ListActivePagedAsync<TKey>(
            Expression<Func<Category, bool>>? predicate = null,
            Expression<Func<Category, TKey>>? orderBy = null,
            bool ascending = true,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default,
            params Expression<Func<Category, object>>[] includes)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            return await query
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountActiveAsync(
            Expression<Func<Category, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .CountAsync(cancellationToken);
        }

        public async Task<bool> ExistsByTitleAsync(
            string title,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c =>
                    c.Title == title &&
                    !c.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    c => c.Id != excludeId.Value);
            }

            return await query
                .AnyAsync(cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> query = _dbSet
                .AsNoTracking()
                .Where(c =>
                    c.Slug == slug &&
                    !c.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    c => c.Id != excludeId.Value);
            }

            return await query
                .AnyAsync(cancellationToken);
        }
    }
}