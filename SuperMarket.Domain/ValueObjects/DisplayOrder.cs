using System;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class DisplayOrder : ValueObject
    {
        public int Value { get; private init; }

        private DisplayOrder() { } // EF Core

        private DisplayOrder(int value)
        {
            if (value < 0)
                throw new ArgumentException("Display order cannot be negative.", nameof(value));

            Value = value;
        }

        public static DisplayOrder Create(int value) => new DisplayOrder(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
