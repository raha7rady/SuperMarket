using System;
using System.Collections.Generic;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class Name : ValueObject
    {
        public string FirstName { get; }
        public string LastName { get; }

        private Name() { } // EF Core

        private Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public static Name Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 50)
                throw new ArgumentException("First name cannot exceed 50 characters.", nameof(firstName));
            if (lastName.Length > 50)
                throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(lastName));

            return new Name(firstName, lastName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
