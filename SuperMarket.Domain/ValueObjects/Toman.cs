using System;

namespace SuperMarket.Domain.ValueObjects
{
    /// <summary>
    /// Represents a monetary value in Iranian Toman as a Value Object.
    /// Immutable, validated, and supports basic arithmetic.
    /// </summary>
    public sealed class Toman : ValueObject, IEquatable<Toman>
    {
        public decimal Amount { get; private init; }

        private Toman() { } // EF Core

        private Toman(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));

            Amount = Math.Round(amount, 0); // تومان بدون اعشار
        }

        public static Toman Create(decimal amount) => new Toman(amount);

        public Toman Add(Toman other) => new Toman(this.Amount + other.Amount);

        public Toman Subtract(Toman other)
        {
            if (other.Amount > this.Amount)
                throw new InvalidOperationException("Resulting amount cannot be negative.");
            return new Toman(this.Amount - other.Amount);
        }

        public override string ToString() => $"{Amount:N0} تومان";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }

        public bool Equals(Toman? other) => other is not null && Amount == other.Amount;

        public Toman Multiply(int factor)
        {
            if (factor < 0)
                throw new ArgumentException("Multiplier cannot be negative.", nameof(factor));

            return new Toman(Amount * factor);
        }

    }
}
