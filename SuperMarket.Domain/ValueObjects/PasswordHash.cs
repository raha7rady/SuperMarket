using System;
using System.Collections.Generic;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class PasswordHash : ValueObject
    {
        public string Value { get; }

        private PasswordHash()
        {
            Value = null!; // EF materialization
        }

        private PasswordHash(string value)
        {
            Value = value;
        }

        public static PasswordHash Create(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash is required.", nameof(hash));

            if (hash.Length < 32)
                throw new ArgumentException("Password hash appears invalid.", nameof(hash));

            return new PasswordHash(hash);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}