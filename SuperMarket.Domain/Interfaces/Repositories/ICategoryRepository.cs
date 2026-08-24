
using System.Linq.Expressions;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository
        : IRepository<Category, Guid>
    {
        IQueryable<Category> QueryAll();

        Task<Category?> GetWithProductsAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default);

        Task<Category?> GetByTitleAsync(
            string title,
            CancellationToken cancellationToken = default);

        Task<Category?> GetByTitleAsync(
            string title,
            Guid? excludeId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Category>> ListActiveAsync(
            Expression<Func<Category, bool>>? predicate = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<Category, object>>[] includes);

        Task<IReadOnlyList<Category>> ListActivePagedAsync<TKey>(
            Expression<Func<Category, bool>>? predicate = null,
            Expression<Func<Category, TKey>>? orderBy = null,
            bool ascending = true,
            int skip = 0,
            int take = 20,
            CancellationToken cancellationToken = default,
            params Expression<Func<Category, object>>[] includes);

        Task<int> CountActiveAsync(
            Expression<Func<Category, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByTitleAsync(
            string title,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);
    }
}