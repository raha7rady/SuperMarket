using System;
using System.Collections.Generic;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class CartQuantity : ValueObject
    {
        public int Value { get; private init; }

        private CartQuantity() { } // EF Core

        private CartQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(value));

            Value = value;
        }

        public static CartQuantity Create(int value) => new CartQuantity(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();


    }
}
