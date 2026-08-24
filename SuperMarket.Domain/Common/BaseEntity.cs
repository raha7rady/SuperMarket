

using System;

namespace SuperMarket.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public override bool Equals(object obj)
        {
            if (obj is not BaseEntity other) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BaseEntity left, BaseEntity right) => !(left == right);
    }
}

