using System;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class ProductTitle : ValueObject
    {
        public string Value { get; private set; } = null!;

        private ProductTitle() { } // EF Core

        private ProductTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title is required.", nameof(value));

            if (value.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(value));

            Value = value.Trim();
        }

        public static ProductTitle Create(string value) => new ProductTitle(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
