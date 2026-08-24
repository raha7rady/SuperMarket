using System;
using SuperMarket.Domain.Common;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    public class OrderItem : AuditableEntity
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }

        public ProductTitle Title { get; private set; } = null!;
        public Toman Price { get; private set; }
        public StockQuantity Quantity { get; private set; }

        public Toman SubTotal => Price.Multiply(Quantity.Value);

        private OrderItem() { }

        internal OrderItem(Guid orderId, Guid productId, string title, decimal price, int quantity)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId is required.", nameof(orderId));

            OrderId = orderId;
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

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Product title required.", nameof(title));

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            ProductId = productId;
            Title = ProductTitle.Create(title);
            Price = Toman.Create(price);
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            Quantity = StockQuantity.Create(quantity);
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted order item.");
        }

        public override void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;
            base.SoftDelete(deletedBy);
        }
    }
}
