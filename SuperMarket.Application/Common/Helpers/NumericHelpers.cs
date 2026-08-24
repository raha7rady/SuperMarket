using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Application.Common.Helpers
{
    /// <summary>
    /// Helper for safe numeric operations
    /// </summary>
    public static class NumericHelpers
    {
        /// <summary>
        /// Returns the maximum value from a numeric sequence safely.
        /// Throws ArgumentException if sequence is empty or not numeric.
        /// </summary>
        public static T MaxSafe<T>(IEnumerable<T> source) where T : IComparable<T>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            T maxValue = default!;
            bool hasValue = false;

            foreach (var item in source)
            {
                if (!IsNumericType(item))
                    throw new ArgumentException($"Type {typeof(T)} is not numeric.");

                if (!hasValue)
                {
                    maxValue = item;
                    hasValue = true;
                }
                else if (item.CompareTo(maxValue) > 0)
                {
                    maxValue = item;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements.");

            return maxValue;
        }

        private static bool IsNumericType<T>(T item)
        {
            var type = typeof(T);
            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}