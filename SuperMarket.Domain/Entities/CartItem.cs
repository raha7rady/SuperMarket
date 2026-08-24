
using System;
using SuperMarket.Domain.Common;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    public class CartItem : AuditableEntity
    {
        public Guid CartId { get; private set; }
        public Cart Cart { get; private set; } = null!;

        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }

        public ProductTitle Title { get; private set; } = null!;
        public Toman Price { get; private set; }
        public CartQuantity Quantity { get; private set; }

        public Toman SubTotal => Price.Multiply(Quantity.Value);

        private CartItem() { }

        internal CartItem(Guid cartId, Guid productId, string title, decimal price, int quantity)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentException("CartId is required.", nameof(cartId));

            CartId = cartId;
            SetProduct(productId, title, price);
            SetQuantity(quantity);
        }

        internal void Increase(int quantity)
        {
            EnsureNotDeleted();
            SetQuantity(Quantity.Value + quantity);
        }

        internal void ChangeQuantity(int quantity)
        {
            EnsureNotDeleted();
            SetQuantity(quantity);
        }

        private void SetProduct(Guid productId, string title, decimal price)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Invalid productId.", nameof(productId));

            ProductId = productId;
            Title = ProductTitle.Create(title);
            Price = Toman.Create(price);
        }

        private void SetQuantity(int quantity)
        {
            Quantity = CartQuantity.Create(quantity);
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted cart item.");
        }

        public override void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;
            base.SoftDelete(deletedBy);
        }
    }
}
