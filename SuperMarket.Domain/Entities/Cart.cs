
using SuperMarket.Domain.Common;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    public class Cart : AuditableEntity
    {
        private const int MaxQuantityPerItem = 100;

        private readonly List<CartItem> _items = new();

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public IReadOnlyCollection<CartItem> Items => _items;
        public bool HasItems => ActiveItems.Any();
        public int TotalItems => ActiveItems.Sum(i => i.Quantity.Value);
        public Toman TotalAmount => ActiveItems.Aggregate(Toman.Create(0), (t, i) => t.Add(i.SubTotal));

        private IEnumerable<CartItem> ActiveItems => _items.Where(i => !i.IsDeleted);

        private Cart() { }

        public Cart(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.", nameof(userId));

            UserId = userId;
        }

        // ✅ تغییر int به Guid
        public void AddItem(Guid productId, string title, decimal price, int quantity)
        {
            EnsureNotDeleted();
            ValidateProduct(productId, title, price, quantity);

            var item = ActiveItems.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
            {
                _items.Add(new CartItem(Id, productId, title, price, quantity));
                return;
            }

            var newQuantity = item.Quantity.Value + quantity;

            if (newQuantity > MaxQuantityPerItem)
                throw new InvalidOperationException("Maximum allowed quantity exceeded.");

            item.ChangeQuantity(newQuantity);
        }

        public void ChangeItemQuantity(Guid productId, int quantity)
        {
            EnsureNotDeleted();

            if (quantity <= 0 || quantity > MaxQuantityPerItem)
                throw new ArgumentException("Invalid quantity.", nameof(quantity));

            GetActiveItem(productId).ChangeQuantity(quantity);
        }

        // ✅ اضافه کردن performedBy
        public void RemoveItem(Guid productId, Guid performedBy)
        {
            EnsureNotDeleted();
            GetActiveItem(productId).SoftDelete(performedBy);
        }

        public void Clear(Guid performedBy)
        {
            EnsureNotDeleted();

            foreach (var item in ActiveItems.ToList())
                item.SoftDelete(performedBy);
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

        private CartItem GetActiveItem(Guid productId) =>
            ActiveItems.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("Item not found in cart.");

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted cart.");
        }

        private static void ValidateProduct(Guid productId, string title, decimal price, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Invalid productId.", nameof(productId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Product title is required.", nameof(title));

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

    }
}
