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
/// Represents a point in a two-dimensional plane defined by an ordered pair of floating-point x- and y-coordinates.
/// </summary>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct PointF : IEquatable<PointF>
{
    /// <summary>
    /// Represents an empty <see cref="PointF"/> with all member data left uninitialized.
    /// </summary>
    public static readonly PointF Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PointF"/> class.
    /// </summary>
    /// <param name="vector">A vector whose two floating-point values will be used as coordinates.</param>
    public PointF(Vector2 vector)
        : this(vector.X, vector.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointF"/> class.
    /// </summary>
    /// <param name="point">A point whose two integer coordinates will be used.</param>
    public PointF(Point point)
        : this(point.X, point.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointF"/> class.
    /// </summary>
    /// <param name="x">The x-coordinate of this point.</param>
    /// <param name="y">The y-coordinate of this point.</param>
    public PointF(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the x-coordinate of this point.
    /// </summary>
    public float X
    { get; }

    /// <summary>
    /// Gets the y-coordinate of this point.
    /// </summary>
    public float Y
    { get; }

    /// <summary>
    /// Gets a value indicating whether this point is <see cref="Empty"/>.
    /// </summary>
    /// <remarks>
    /// The .NET runtime employs an inconsistently followed convention as far as <c>IsEmpty</c>-like properties
    /// for value types are concerned. Within Bad Echo frameworks, a value is considered empty if equal to one
    /// with all of its member data left uninitialized.
    /// </remarks>
    public bool IsEmpty
        => Equals(Empty);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Point"/> value to a <see cref="PointF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    public static implicit operator PointF(Point point)
        => FromPoint(point);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Vector2"/> value to a <see cref="PointF"/> value.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    public static implicit operator PointF(Vector2 vector)
        => FromVector2(vector);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="PointF"/> value to a <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    public static implicit operator Vector2(PointF point)
        => ToVector2(point);

    /// <summary>
    /// Determines whether two <see cref="PointF"/> values have the same coordinates.
    /// </summary>
    /// <param name="left">The first point to compare.</param>
    /// <param name="right">The second point to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same point in a two-dimensional plane as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(PointF left, PointF right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PointF"/> values have different coordinates.
    /// </summary>
    /// <param name="left">The first point to compare.</param>
    /// <param name="right">The second point to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same point in a two-dimensional plane as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(PointF left, PointF right)
        => !left.Equals(right);

    /// <summary>
    /// Translates the <see cref="PointF"/> value by the <see cref="Size"/> value.
    /// </summary>
    /// <param name="left">The point which <c>right</c> translates.</param>
    /// <param name="right">The size that specifies the displacement amounts to be added to <c>left</c>.</param>
    /// <returns><c>left</c> translated by <c>right</c>.</returns>
    public static PointF operator +(PointF left, Size right)
        => left.Add(right);

    /// <summary>
    /// Translates the <see cref="PointF"/> value by the negative of the <see cref="Size"/> value.
    /// </summary>
    /// <param name="left">The point which <c>right</c> translates.</param>
    /// <param name="right">
    /// The size that specifies the displacement amounts to be subtracted from <c>left</c>.
    /// </param>
    /// <returns><c>left</c> translated by <c>right</c>.</returns>
    public static PointF operator -(PointF left, Size right)
        => left.Subtract(right);

    /// <summary>
    /// Converts the specified <see cref="Point"/> value to an equivalent <see cref="PointF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <returns>A <see cref="PointF"/> value equivalent to <c>point</c>.</returns>
    public static PointF FromPoint(Point point)
        => new(point);

    /// <summary>
    /// Converts the specified <see cref="Vector2"/> value to an equivalent <see cref="PointF"/> value.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A <see cref="PointF"/> value equivalent to <c>vector</c>.</returns>
    public static PointF FromVector2(Vector2 vector)
        => new(vector);

    /// <summary>
    /// Converts the specified <see cref="PointF"/> value to an equivalent <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <returns>A <see cref="Vector2"/> value equivalent to <c>point</c>.</returns>
    public static Vector2 ToVector2(PointF point) 
        => new(point.X, point.Y);

    /// <summary>
    /// Determines whether two <see cref="PointF"/> values have the same coordinates.
    /// </summary>
    /// <param name="first">The first point to compare.</param>
    /// <param name="second">The second point to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same point in a two-dimensional plane as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(PointF first, PointF second)
        => first.Equals(second);

    /// <inheritdoc/>
    public override bool Equals(object? obj) 
        => obj is PointF other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(X, Y);

    /// <inheritdoc/>
    public override string ToString()
        => $"X: {X}, Y: {Y}";

    /// <inheritdoc/>
    public bool Equals(PointF other)
        => X.ApproximatelyEquals(other.X) && Y.ApproximatelyEquals(other.Y);

    /// <summary>
    /// Calculates the vector distance from this point to the specified <see cref="PointF"/> value.
    /// </summary>
    /// <param name="other">The point to calculate the vector distance to.</param>
    /// <returns>The displacement vector from this point to <c>other</c>.</returns>
    public Vector2 Displace(PointF other)
    {
        Vector2 otherVector = other;

        return otherVector - this;
    }

    /// <summary>
    /// Translates this point by the specified <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">
    /// The size that specifies the displacement amounts to be added to this point's coordinates.
    /// </param>
    /// <returns>This point translated by <c>size</c>.</returns>
    public PointF Add(Size size)
        => new(X + size.Width, Y + size.Height);

    /// <summary>
    /// Translates this point by the specified <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="size">
    /// The size that specifies the displacement amounts to be added to this point's coordinates.
    /// </param>
    /// <returns>This point translated by <c>size</c>.</returns>
    public PointF Add(SizeF size)
        => new(X + size.Width, Y + size.Height);

    /// <summary>
    /// Translates this point by the negative of the specified <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">
    /// The size that specifies the displacement amounts to be subtracted from this point's coordinates.
    /// </param>
    /// <returns>This point translated by <c>size</c>.</returns>
    public PointF Subtract(Size size)
        => new(X - size.Width, Y - size.Height);

    /// <summary>
    /// Translates this point by the negative of the specified <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="size">
    /// The size that specifies the displacement amounts to be subtracted from this point's coordinates.
    /// </param>
    /// <returns>This point translated by <c>size</c>.</returns>
    public PointF Subtract(SizeF size)
        => new(X - size.Width, Y - size.Height);
}
