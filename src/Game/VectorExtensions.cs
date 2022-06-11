//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a set of static methods that aids in matters related to vectors.
/// </summary>
public static class VectorExtensions
{
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
