using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories.Base;

namespace SuperMarket.Infrastructure.Repositories;

public sealed class WishlistRepository : Repository<WishlistItem, Guid>, IWishlistRepository
{
    public WishlistRepository(SuperMarketDbContext dbContext)
        : base(dbContext)
    {
    }
}
