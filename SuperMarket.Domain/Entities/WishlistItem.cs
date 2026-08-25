using System;
using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities
{
    public class WishlistItem : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        private WishlistItem() { }

        public WishlistItem(Guid userId, Guid productId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required.", nameof(userId));

            if (productId == Guid.Empty)
                throw new ArgumentException("Product id is required.", nameof(productId));

            UserId = userId;
            ProductId = productId;
        }
    }
}
