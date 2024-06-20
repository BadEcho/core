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

namespace BadEcho;

/// <summary>
/// Provides a set of static methods intended to aid in equality comparisons between objects.
/// </summary>
public static class Comparison
{
    /// <summary>
    /// Creates a comparer that compares two objects by executing the provided function.
    /// </summary>
    /// <typeparam name="T">The type of objects to be compared.</typeparam>
    /// <param name="comparison">A function which compares two objects of type <typeparamref name="T"/>.</param>
    /// <returns>A <see cref="IComparer{T}"/> instance which will execute <c>comparison</c>.</returns>
    public static IComparer<T> Using<T>(Func<T, T, int> comparison)
        => new Comparer<T>(comparison);

    /// <summary>
    /// Provides a comparer which performs comparisons using a provided function.
    /// </summary>
    /// <typeparam name="T">The type of objects compared by this comparer.</typeparam>
    private sealed class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T}"/> class.
        /// </summary>
        /// <param name="comparison">A function which compares two objects of type <typeparamref name="T"/>.</param>
        public Comparer(Func<T, T, int> comparison)
        {
            _comparison = comparison;
        }

        /// <inheritdoc/>
        public int Compare(T? x, T? y)
        {
            if (x == null)
                return y == null ? 0 : -1;

            return y == null ? 1 : _comparison(x, y);
        }
    }
}
