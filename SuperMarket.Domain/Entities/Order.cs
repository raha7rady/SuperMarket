using System;
using System.Collections.Generic;
using System.Linq;
using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    public class Order : AuditableEntity
    {
        private const int MaxItemsPerOrder = 100;
        private const int MaxQuantityPerItem = 100;

        private readonly List<OrderItem> _items = new();

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public OrderStatus OrderStatus { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items;
        private IEnumerable<OrderItem> ActiveItems => _items.Where(i => !i.IsDeleted);

        public bool HasItems => ActiveItems.Any();

        public Toman TotalPrice =>
            ActiveItems.Aggregate(Toman.Create(0), (t, i) => t.Add(i.SubTotal));

        private Order() { }

        public Order(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.", nameof(userId));

            UserId = userId;
            OrderStatus = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
        }

        public void AddItem(Guid productId, string title, decimal price, int quantity, Guid performedBy)
        {
            EnsureModifiable();
            ValidateItem(productId, title, price, quantity);

            var item = ActiveItems.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
            {
                if (ActiveItems.Count() >= MaxItemsPerOrder)
                    throw new InvalidOperationException("Maximum items per order exceeded.");

                _items.Add(new OrderItem(Id, productId, title, price, quantity));
            }
            else
            {
                var newQuantity = item.Quantity.Value + quantity;

                if (newQuantity > MaxQuantityPerItem)
                    throw new InvalidOperationException("Maximum quantity per item exceeded.");

                item.ChangeQuantity(newQuantity);
            }

            SetModified(performedBy);
        }

        public void RemoveItem(Guid productId, Guid performedBy)
        {
            EnsureModifiable();
            GetActiveItem(productId).SoftDelete(performedBy);
            SetModified(performedBy);
        }

        public void ChangeItemQuantity(Guid productId, int quantity, Guid performedBy)
        {
            EnsureModifiable();

            if (quantity <= 0 || quantity > MaxQuantityPerItem)
                throw new ArgumentException("Invalid quantity.", nameof(quantity));

            GetActiveItem(productId).ChangeQuantity(quantity);
            SetModified(performedBy);
        }

        public void MarkAsPaid(Guid performedBy)
        {
            EnsureNotDeleted();

            if (!HasItems)
                throw new InvalidOperationException("Cannot pay an order with no items.");

            if (PaymentStatus == PaymentStatus.Paid)
                throw new InvalidOperationException("Order already paid.");

            if (OrderStatus == OrderStatus.Canceled)
                throw new InvalidOperationException("Canceled order cannot be paid.");

            PaymentStatus = PaymentStatus.Paid;
            OrderStatus = OrderStatus.Processing;

            SetModified(performedBy);
        }

        public void MarkAsShipped(Guid performedBy)
        {
            EnsureNotDeleted();

            if (OrderStatus != OrderStatus.Processing)
                throw new InvalidOperationException("Only processing orders can be shipped.");

            OrderStatus = OrderStatus.Shipped;
            SetModified(performedBy);
        }

        public void MarkAsDelivered(Guid performedBy)
        {
            EnsureNotDeleted();

            if (OrderStatus != OrderStatus.Shipped)
                throw new InvalidOperationException("Only shipped orders can be delivered.");

            OrderStatus = OrderStatus.Delivered;
            SetModified(performedBy);
        }

        public void Cancel(Guid performedBy)
        {
            EnsureNotDeleted();

            if (OrderStatus is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Returned)
                throw new InvalidOperationException("Order cannot be canceled.");

            OrderStatus = OrderStatus.Canceled;

            if (PaymentStatus == PaymentStatus.Paid)
                PaymentStatus = PaymentStatus.Failed;

            SetModified(performedBy);
        }

        public void MarkAsRefunded(Guid performedBy)
        {
            EnsureNotDeleted();

            if (PaymentStatus != PaymentStatus.Paid)
                throw new InvalidOperationException("Only paid orders can be refunded.");

            PaymentStatus = PaymentStatus.Refunded;
            SetModified(performedBy);
        }

        public override void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;

            base.SoftDelete(deletedBy);

            foreach (var item in ActiveItems.ToList())
                item.SoftDelete(deletedBy);
        }

        public void Restore(Guid restoredBy)
        {
            if (!IsDeleted) return;

            base.Restore();

            foreach (var item in _items.Where(i => i.IsDeleted))
                item.Restore();

            SetModified(restoredBy);
        }

        private OrderItem GetActiveItem(Guid productId) =>
            ActiveItems.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("Item not found.");

        private void EnsureModifiable()
        {
            EnsureNotDeleted();

            if (OrderStatus != OrderStatus.Pending)
                throw new InvalidOperationException("Order can no longer be modified.");
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted order.");
        }

        private static void ValidateItem(Guid productId, string title, decimal price, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Invalid productId.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Product title required.");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
        }
    }
}
