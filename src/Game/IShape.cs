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
/// Defines the external spatial boundary of an entity.
/// </summary>
public interface IShape
{
    /// <summary>
    /// Gets the coordinates of the center of this shape.
    /// </summary>
    PointF Center { get; }

    /// <summary>
    /// Gets the size of this shape.
    /// </summary>
    SizeF Size { get; }
    
    /// <summary>
    /// Determines if the specified point is contained within this shape.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if <c>point</c> is contained within this shape; otherwise, false.</returns>
    bool Contains(PointF point);

    /// <summary>
    /// Calculates the penetration vector formed by this shape overlapping with the specified shape.
    /// </summary>
    /// <param name="other">A shape overlapping with this shape.</param>
    /// <returns>The penetrating vector formed by this shape overlapping with <c>other</c>.</returns>
    /// <remarks>
    /// The penetration vector describes the depth of penetration between the two shapes. Adding the vector
    /// to the position of this shape will separate it from <c>other</c> such that they are only touching each
    /// other rather than intersecting.
    /// </remarks>
    Vector2 CalculatePenetration(IShape other);

    /// <summary>
    /// Calculates the penetration vector formed by this shape overlapping with the specified rectangle.
    /// </summary>
    /// <param name="other">A rectangle overlapping with this shape.</param>
    /// <returns>The penetrating vector formed by this shape overlapping with <c>other</c>.</returns>
    /// <remarks>
    /// The penetration vector describes the depth of penetration between the two shapes. Adding the vector
    /// to the position of this shape will separate it from <c>other</c> such that they are only touching each
    /// other rather than intersecting.
    /// </remarks>
    Vector2 CalculatePenetration(RectangleF other);

    /// <summary>
    /// Calculates the penetration vector formed by this shape overlapping with the specified circle.
    /// </summary>
    /// <param name="other">A circle overlapping with this shape.</param>
    /// <returns>The penetrating vector formed by this shape overlapping with <c>other</c>.</returns>
    /// <remarks>
    /// The penetration vector describes the depth of penetration between the two shapes. Adding the vector
    /// to the position of this shape will separate it from <c>other</c> such that they are only touching each
    /// other rather than intersecting.
    /// </remarks>
    Vector2 CalculatePenetration(Circle other);

    /// <summary>
    /// Gets the coordinates for the location on this shape nearest to the specified point.
    /// </summary>
    /// <param name="point">The point to find the nearest location on this shape to.</param>
    /// <returns>The coordinates for the nearest location on this shape to <c>point</c>.</returns>
    PointF GetPointClosestTo(PointF point);

    /// <summary>
    /// Creates a copy of this <see cref="IShape"/> instance centered at the specified coordinates.
    /// </summary>
    /// <param name="center">The coordinates of the center for the new shape.</param>
    /// <returns>A shape with the same dimensions as this, centered at <paramref name="center"/>.</returns>
    IShape CenterAt(PointF center);

    /// <summary>
    /// Determines if this shape intersects with the one specified.
    /// </summary>
    /// <param name="other">The other shape to check.</param>
    /// <returns>True if <c>other</c> intersects with this shape; otherwise, false.</returns>
    bool Intersects(IShape other)
    {
        Require.NotNull(other, nameof(other));
        
        PointF closestPoint = other.GetPointClosestTo(Center);

        return Contains(closestPoint);
    }
}
