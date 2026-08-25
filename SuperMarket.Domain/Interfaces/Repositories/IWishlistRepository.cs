using System;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Domain.Interfaces.Repositories
{
    public interface IWishlistRepository : IRepository<WishlistItem, Guid>
    {
    }
}
