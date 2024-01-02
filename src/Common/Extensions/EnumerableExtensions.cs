//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;

namespace BadEcho.Extensions;

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

    /// <summary>
    /// Filters a sequence for all strings neither null nor empty.
    /// </summary>
    /// <param name="source">The sequence of strings to filter null and empty values out of.</param>
    /// <returns><c>source</c> with null and empty values removed.</returns>
    public static IEnumerable<string> WhereNotNullOrEmpty(this IEnumerable<string?> source)
    {
        return source.WhereNotNull()
                     .Where(x => x.Length != 0);
    }
}