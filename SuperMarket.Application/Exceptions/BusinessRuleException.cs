
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Application.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public BusinessRuleException(string error)
            : this(new[] { error })
        {
        }

        public BusinessRuleException(IEnumerable<string> errors)
            : base("One or more business rules were violated.")
        {
            if (errors == null || !errors.Any())
                throw new ArgumentException("Business rule errors cannot be empty.", nameof(errors));

            Errors = errors.ToList().AsReadOnly();
        }

        public BusinessRuleException(string error, Exception innerException)
            : this(new[] { error })
        {
            if (innerException != null)
                throw new Exception("Inner exception provided", innerException);
        }
    }
}
