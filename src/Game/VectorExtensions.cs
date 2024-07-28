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

using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to vectors.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// Converts this vector value into one with the same direction, but with a length of one.
    /// </summary>
    /// <param name="source">The vector value to convert.</param>
    /// <returns>A normalized value for <c>source</c>, otherwise known as the unit vector.</returns>
    /// <remarks>
    /// This extension method exists because the built-in vector normalization method modifies the instance's
    /// actual value. Using this method allows us to get the unit vector without modifying the value of <c>source</c>.
    /// </remarks>
    public static Vector2 ToUnit(this Vector2 source)
    {
        source.Normalize();
        
        return source;
    }

    /// <summary>
    /// Converts this vector value into a <see cref="MovementDirection"/> value specifying the vector's direction of movement.
    /// </summary>
    /// <param name="source">The vector value to convert.</param>
    /// <returns>A <see cref="MovementDirection"/> value that specifies the movement direction of <c>source</c>.</returns>
    public static MovementDirection ToDirection(this Vector2 source)
    {
        if (source == Vector2.Zero)
            return MovementDirection.None;

        return Math.Abs(source.X) > Math.Abs(source.Y)
            ? source.X > 0 ? MovementDirection.Right : MovementDirection.Left
            : source.Y > 0 ? MovementDirection.Down : MovementDirection.Up;
    }

    /// <summary>
    /// Returns a new vector whose elements are absolute values of this vector's elements.
    /// </summary>
    /// <param name="source">The source vector.</param>
    /// <returns>The absolute value vector.</returns>
    public static Vector2 Abs(this Vector2 source)
        => new(Math.Abs(source.X), Math.Abs(source.Y));

    /// <summary>
    /// Returns a value that indicates if any element in this vector is less than the corresponding element in another vector.
    /// </summary>
    /// <param name="source">The first vector to compare.</param>
    /// <param name="other">The second vector to compare.</param>
    /// <returns>
    /// True if any element in <c>source</c> is less than the corresponding element in <c>other</c>; otherwise, false.
    /// </returns>
    public static bool LessThanAny(this Vector2 source, Vector2 other)
        => source.X < other.X || source.Y < other.Y;

    /// <summary>
    /// Returns a value that indicates if any element in this vector is greater than the corresponding element in another vector.
    /// </summary>
    /// <param name="source">The first vector to compare.</param>
    /// <param name="other">The second vector to compare.</param>
    /// <returns>
    /// True if any element in <c>source</c> is greater than the corresponding element in <c>other</c>; otherwise, false.
    /// </returns>
    public static bool GreaterThanAny(this Vector2 source, Vector2 other)
        => source.X > other.X || source.Y > other.Y;

    /// <summary>
    /// Returns a vector with the same direction as this vector, but with a length of one.
    /// </summary>
    /// <param name="source">The source vector.</param>
    /// <returns>The normalized vector.</returns>
    public static Vector2 Normalized(this Vector2 source)
    {   // The vector will have been passed by value, so the original value will not be modified from here.
        source.Normalize();

        return source;
    }
}
