// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to weak references.
/// </summary>
/// <remarks>
/// The methods in this class dealing with null targets exist because even though it is valid to set a weak reference's 
/// target to null, the target parameter in the relevant methods annoyingly isn't marked as nullable. 
/// So, rather keep the use of the null-forgiving operator to a single place.
/// </remarks>
public static class WeakReferenceExtensions
{
    /// <summary>
    /// Creates a <see cref="WeakReference{T}"/> instance with no target set.
    /// </summary>
    /// <typeparam name="T">The type of object that will be referenced.</typeparam>
    /// <returns>A <see cref="WeakReference{T}"/> instance with no target set.</returns>
    public static WeakReference<T> WithNullTarget<T>()
        where T : class
    {
        return new WeakReference<T>(null!);
    }

    /// <summary>
    /// Sets the target of the weak reference to null.
    /// </summary>
    /// <typeparam name="T">The type of object referenced.</typeparam>
    /// <param name="source">The <see cref="WeakReference{T}"/> instance whose target should be set to null.</param>
    public static void SetNullTarget<T>(this WeakReference<T> source)
        where T : class
    {
        Require.NotNull(source, nameof(source));

        source.SetTarget(null!);
    }
}
