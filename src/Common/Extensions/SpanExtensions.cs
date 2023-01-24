//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to representations of contiguous regions of
/// arbitrary memory.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Copies the source <see cref="Span{T}"/> to the destination <see cref="Span{T}"/>, truncating the source if necessary and
    /// then terminating it with null.
    /// </summary>
    /// <param name="source">The span to copy from.</param>
    /// <param name="destination">The span to copy to.</param>
    public static void CopyToAndTerminate(this ReadOnlySpan<char> source, Span<char> destination)
    {
        if (source.Length >= destination.Length)
            source = source[..(destination.Length - 1)];

        source.CopyTo(destination);

        destination[source.Length] = '\0';
    }

    /// <summary>
    /// Slices the <see cref="Span{T}"/> at the first null found.
    /// </summary>
    /// <param name="span">The span to slice.</param>
    /// <returns>
    /// A span that consists of length elements from <c>span</c> to the first null found; if no null is found, then the
    /// current span is returned
    /// </returns>
    public static Span<char> SliceAtFirstNull(this Span<char> span)
    {
        int index = span.IndexOf('\0');
        
        return index == -1 ? span : span[..index];
    }
}
