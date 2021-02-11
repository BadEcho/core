//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BadEcho.Odin.Extensions
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to <see cref="IEnumerable"/> objects.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Filters a sequence for all values that are not null.
        /// </summary>
        /// <typeparam name="T">The type of items enumerated.</typeparam>
        /// <param name="source">The sequence to filter null values out of.</param>
        /// <returns><c>source</c> with null values removed.</returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        {
            return source.Where(x => x != null)
                         .Select(x => x!);
        }
    }
}
