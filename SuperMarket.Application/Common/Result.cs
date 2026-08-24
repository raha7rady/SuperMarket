using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Application.Common
{
    public class Result
    {
        private static readonly IReadOnlyList<string> EmptyErrors = Array.Empty<string>();

        protected Result(bool isSuccess, IReadOnlyList<string> errors, string? errorCode)
        {
            if (isSuccess && errors.Count > 0)
                throw new InvalidOperationException("A successful result cannot contain errors.");

            if (!isSuccess && errors.Count == 0)
                throw new InvalidOperationException("A failed result must contain at least one error.");

            IsSuccess = isSuccess;
            Errors = errors;
            ErrorCode = errorCode;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public IReadOnlyList<string> Errors { get; }

        public string? ErrorCode { get; }

        public string? FirstError => Errors.FirstOrDefault();

        public static Result Success()
            => new Result(true, EmptyErrors, null);

        public static Result Failure(string error, string? errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be empty.", nameof(error));

            return new Result(false, new[] { error }, errorCode);
        }

        public static Result Failure(IEnumerable<string> errors, string? errorCode = null)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var errorList = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (errorList.Count == 0)
                throw new ArgumentException("At least one valid error message must be provided.", nameof(errors));

            return new Result(false, errorList.AsReadOnly(), errorCode);
        }
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        protected Result(T value)
            : base(true, Array.Empty<string>(), null)
        {
            _value = value;
        }

        protected Result(IReadOnlyList<string> errors, string? errorCode)
            : base(false, errors, errorCode)
        {
            _value = default;
        }

        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException("Cannot access the value of a failed result.");
                return _value!;
            }
        }

        public static Result<T> Success(T value)
            => new Result<T>(value);

        public static Result<T> Failure(string error, string? errorCode = null)
            => new Result<T>(new[] { error }, errorCode);

        public static Result<T> Failure(IEnumerable<string> errors, string? errorCode = null)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var errorList = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (errorList.Count == 0)
                throw new ArgumentException("At least one valid error message must be provided.", nameof(errors));

            return new Result<T>(errorList.AsReadOnly(), errorCode);
        }
    }
}
