using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        private Email()
        {
            Value = null!;
        }

        private Email(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            Value = value;
        }

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            email = email.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Invalid email format.", nameof(email));

            return new Email(email);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
