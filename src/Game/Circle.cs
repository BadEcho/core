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

using BadEcho.Extensions;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Represents a circle defined by a set of floating-point numbers for its center point and radius.
/// </summary>
/// <remarks>
/// The overly verbose mathematical documentation is present because...well, why not? But, it's here also for my own
/// memory
/// </remarks>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct Circle : IEquatable<Circle>, IShape
{
    /// <summary>
    /// Represents an empty <see cref="Circle"/> with all member data left uninitialized.
    /// </summary>
    public static readonly Circle Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Circle"/> class.
    /// </summary>
    /// <param name="center">The coordinates of the center of this circle.</param>
    /// <param name="radius">The distance from the center of this circle to its perimeter.</param>
    public Circle(PointF center, float radius)
        : this(center.X, center.Y, radius)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Circle"/> class.
    /// </summary>
    /// <param name="x">The x-coordinate of the center of this circle.</param>
    /// <param name="y">The y-coordinate of the center of this circle.</param>
    /// <param name="radius">The distance from the center of this circle to its perimeter.</param>
    public Circle(float x, float y, float radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    /// <summary>
    /// Gets the x-coordinate of the center of this circle.
    /// </summary>
    public float X
    { get; }

    /// <summary>
    /// Gets the y-coordinate of the center of this circle.
    /// </summary>
    public float Y
    { get; }

    /// <summary>
    /// Gets the distance from the center of this circle to its perimeter.
    /// </summary>
    public float Radius
    { get; }

    /// <inheritdoc/>
    public PointF Center
        => new(X, Y);

    /// <summary>
    /// Gets a value indicating whether this circle is <see cref="Empty"/>.
    /// </summary>
    /// <remarks>
    /// The .NET runtime employs an inconsistently followed convention as far as <c>IsEmpty</c>-like properties
    /// for value types are concerned. Within Bad Echo frameworks, a value is considered empty if equal to one
    /// with all of its member data left uninitialized.
    /// </remarks>
    public bool IsEmpty
        => Equals(Empty);

    /// <summary>
    /// Determines whether two <see cref="Circle"/> values have the same location and radius.
    /// </summary>
    /// <param name="left">The first circle to compare.</param>
    /// <param name="right">The second circle to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same circular region as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(Circle left, Circle right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Circle"/> values have a different location or radius.
    /// </summary>
    /// <param name="left">The first circle to compare.</param>
    /// <param name="right">The second circle to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same circular region as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(Circle left, Circle right)
        => !left.Equals(right);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Circle other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(X, Y, Radius);

    /// <inheritdoc/>
    public override string ToString()
        => $"X: {X}, Y: {Y}, Radius: {Radius}";

    /// <inheritdoc/>
    public bool Equals(Circle other)
        => X.ApproximatelyEquals(other.X)
            && Y.ApproximatelyEquals(other.Y)
            && Radius.ApproximatelyEquals(other.Radius);

    /// <summary>
    /// Determines if the specified point is contained within this circle.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if <c>point</c> is contained within this circle; otherwise, false.</returns>
    /// <remarks>
    /// A point is considered to be within this circle if the displacement vector from this circle's center to <c>point</c>
    /// has a length that does not exceed this circle's radius.
    /// </remarks>
    public bool Contains(PointF point)
    {
        Vector2 distance = Center.Displace(point);

        return distance.Length() <= Radius;
    }

    /// <summary>
    /// Determines if the specified circle is wholly contained within this circle.
    /// </summary>
    /// <param name="other">The circle to check.</param>
    /// <returns>True if <c>other</c> is wholly contained within this circle; otherwise, false.</returns>
    /// <remarks>
    /// A circle is considered to be within this circle if the displacement vector from this circle's center to the center of
    /// <c>other</c> has a length that doesn't exceed the sum of both circles' radii.
    /// </remarks>
    public bool Contains(Circle other)
    {
        Vector2 distance = Center.Displace(other.Center);

        return distance.Length() <= Radius + other.Radius;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The nearest point on the circle is determined by first taking the displacement vector from this circle's center to
    /// <c>point</c> and then normalizing it so we can scale the vector to the distance defined by this circle's radius.
    /// This will give us a vector that, when added to this circle's center, will extend from the center of the circle to a
    /// point on its perimeter in the direction of <c>point</c>.
    /// </remarks>
    public PointF GetPointClosestTo(PointF point)
    {
        Vector2 distance = Center.Displace(point);

        distance.Normalize();

        return Center + Radius * distance;
    }

    /// <summary>
    /// Determines if this circle intersects with the specified circle.
    /// </summary>
    /// <param name="other">The circle to check.</param>
    /// <returns>True if <c>other</c> intersects with this; otherwise, false.</returns>
    /// <remarks>
    /// A circle is considered to intersect with this circle if the displacement vector from this circle's center to the
    /// center of <c>other</c> has a length smaller than the sum of both circles' radii.
    /// </remarks>
    public bool Intersects(Circle other)
    {
        Vector2 distance = Center.Displace(other.Center);

        return distance.Length() < Radius + other.Radius;
    }
}
