

using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Application.Common
{
    public sealed class PagedResult<T> : Result<IReadOnlyList<T>>
    {
        private PagedResult(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
            : base(items)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            if (totalCount < 0)
                throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count cannot be negative.");

            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        private PagedResult(IReadOnlyList<string> errors, string? errorCode)
            : base(errors, errorCode)
        {
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage =>
            IsSuccess && PageNumber > 1;

        public bool HasNextPage =>
            IsSuccess && PageNumber < TotalPages;

        public static PagedResult<T> Success(
            IEnumerable<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var list = items.ToList().AsReadOnly();

            return new PagedResult<T>(list, pageNumber, pageSize, totalCount);
        }

        public static PagedResult<T> Failure(string error, string? errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Error message cannot be empty.", nameof(error));

            return new PagedResult<T>(new[] { error }, errorCode);
        }

        public static PagedResult<T> Failure(IEnumerable<string> errors, string? errorCode = null)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var list = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (list.Count == 0)
                throw new ArgumentException("At least one valid error message must be provided.", nameof(errors));

            return new PagedResult<T>(list.AsReadOnly(), errorCode);
        }
    }
}
