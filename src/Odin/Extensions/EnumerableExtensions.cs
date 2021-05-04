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
        /// Returns the item as a sequence where it is the sole occupant.
        /// </summary>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <param name="source">The item to return as the sole occupant of a sequence.</param>
        /// <returns>A sequence containing only <c>source</c>.</returns>
        /// <remarks>
        /// This extension method creates an array containing the provided item as opposed to using a yield return iterator,
        /// as that is a bit more complicated since it involves the use of compiler generated lambda expression enumerators.
        /// </remarks>
        public static IEnumerable<T> AsEnumerable<T>(this T source) 
            => new[] {source};

        /// <summary>
        /// Checks if the provided sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of items enumerated.</typeparam>
        /// <param name="source">The sequence to check for emptiness.</param>
        /// <returns>True if <c>source</c> is empty; otherwise, false.</returns>
        /// <remarks>
        /// Interestingly, <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/> used to not type check if
        /// source is <see cref="ICollection{T}"/> or <see cref="ICollection"/>. But with .NET Core, it does.
        /// So no need for us to do this optimization any longer. Just a simple call to aforementioned method.
        /// </remarks>
        public static bool IsEmpty<T>(this IEnumerable<T> source) 
            => !source.Any();

        /// <summary>
        /// Determines if the sequence consists of only a single item.
        /// </summary>
        /// <typeparam name="T">The type of items enumerated.</typeparam>
        /// <param name="source">The sequence to check for a single item.</param>
        /// <returns>True if <c>source</c> consists of one and only one item; otherwise, false.</returns>
        public static bool IsSingle<T>(this IEnumerable<T> source) 
            => source.Take(2).Count() == 1;

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
