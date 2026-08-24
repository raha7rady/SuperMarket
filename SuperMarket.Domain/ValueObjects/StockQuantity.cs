using System;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class StockQuantity : ValueObject
    {
        public int Value { get; private init; }

        private StockQuantity() { } // EF Core

        private StockQuantity(int value)
        {
            if (value < 0)
                throw new ArgumentException("Stock quantity cannot be negative.", nameof(value));

            Value = value;
        }

        public static StockQuantity Create(int value) => new StockQuantity(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
