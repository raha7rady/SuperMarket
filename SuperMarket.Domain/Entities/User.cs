
using System;
using System.Collections.Generic;
using System.Linq;
using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    /// <summary>
    /// Domain representation of an application user.
    /// NOTE (architecture): authentication credentials also exist in
    /// ASP.NET Core Identity (see <c>ApplicationUser</c>). Identity is the
    /// single source of truth for sign-in; <see cref="PasswordHash"/> here
    /// is kept only so the Domain/Application layers can work with a user
    /// without depending on Infrastructure/Identity. Every password change
    /// must update both stores — see <c>AccountService</c>.
    /// </summary>
    
    public class User : AuditableEntity
    {
        private readonly List<Order> _orders = new();
        private readonly List<Cart> _carts = new();

        public Name Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }

        public IReadOnlyCollection<Order> Orders => _orders;
        public IReadOnlyCollection<Cart> Carts => _carts;

        private User() { }

        public User(string firstName, string lastName, string email, string passwordHash)
        {
            Name = Name.Create(firstName, lastName);
            Email = Email.Create(email);
            PasswordHash = PasswordHash.Create(passwordHash);
            Role = UserRole.Customer;
        }

        public void ChangeRole(UserRole role, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            if (Role == role)
                return;

            Role = role;

            SetModifiedIfNeeded(modifiedBy);
        }

        public void ChangePassword(string newPasswordHash, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            PasswordHash = PasswordHash.Create(newPasswordHash);
            SetModifiedIfNeeded(modifiedBy);
        }

        public void ChangeName(string firstName, string lastName, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            Name = Name.Create(firstName, lastName);
            SetModifiedIfNeeded(modifiedBy);
        }

        public void ChangeEmail(string email, Guid? modifiedBy = null)
        {
            EnsureNotDeleted();

            Email = Email.Create(email);
            SetModifiedIfNeeded(modifiedBy);
        }

        public void AddOrder(Order order)
        {
            EnsureNotDeleted();

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (_orders.Any(o => o.Id == order.Id))
                return;

            _orders.Add(order);
        }

        public void AddCart(Cart cart)
        {
            EnsureNotDeleted();

            if (cart is null)
                throw new ArgumentNullException(nameof(cart));

            if (_carts.Any(c => c.Id == cart.Id))
                return;

            _carts.Add(cart);
        }

        public override void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;

            base.SoftDelete(deletedBy);

            foreach (var cart in _carts.Where(c => !c.IsDeleted))
                cart.SoftDelete(deletedBy);

            foreach (var order in _orders.Where(o => !o.IsDeleted))
                order.SoftDelete(deletedBy);
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted user.");
        }

        private void SetModifiedIfNeeded(Guid? modifiedBy)
        {
            if (modifiedBy.HasValue)
                SetModified(modifiedBy.Value);
        }
    }
}
