//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
    /// <param name="vector">The vector value to convert.</param>
    /// <returns>A normalized value for <c>vector</c>, otherwise known as the unit vector.</returns>
    /// <remarks>
    /// This extension method exists because the built-in vector normalization method modifies the instance's
    /// actual value. Using this method allows us to get the unit vector without modifying the value of <c>vector</c>.
    /// </remarks>
    public static Vector2 ToUnit(this Vector2 vector)
    {
        vector.Normalize();
        
        return vector;
    }

    /// <summary>
    /// Converts this vector value into a <see cref="MovementDirection"/> value specifying the vector's direction of movement.
    /// </summary>
    /// <param name="vector">The vector value to convert.</param>
    /// <returns>A <see cref="MovementDirection"/> value that specifies the movement direction of <c>vector</c>.</returns>
    public static MovementDirection ToDirection(this Vector2 vector)
    {
        if (vector == Vector2.Zero)
            return MovementDirection.None;

        return Math.Abs(vector.X) > Math.Abs(vector.Y)
            ? vector.X > 0 ? MovementDirection.Right : MovementDirection.Left
            : vector.Y > 0 ? MovementDirection.Down : MovementDirection.Up;
    }
}
