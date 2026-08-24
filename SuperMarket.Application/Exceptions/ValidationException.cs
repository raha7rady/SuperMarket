using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("One or more validation errors occurred.")
        {
            if (errors == null || !errors.Any())
                throw new ArgumentException("Validation errors cannot be empty.", nameof(errors));

            Errors = errors.ToList().AsReadOnly();
        }

        public ValidationException(string error)
            : this(new[] { error })
        {
        }
    }
}
