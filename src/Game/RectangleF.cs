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

using BadEcho.Extensions;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Represents a rectangle defined by a set of floating-point numbers for its upper-left point, width, and height.
/// </summary>
/// <remarks>
/// <para>
/// MonoGame lacks a built-in <see cref="Rectangle"/>-type struct that uses floating-point numbers; this struct
/// seeks to correct this deficiency.
///</para>
/// <para>
/// Support for floating-point numbers when dealing with rectangular regions becomes all the more important when
/// dealing with positional or velocity related <see cref="Vector2"/> values, where inaccuracies will otherwise
/// surface due to value truncation experienced during float-to-integer conversions.
/// </para>
/// </remarks>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct RectangleF : IEquatable<RectangleF>, IShape
{
    /// <summary>
    /// Represents an empty <see cref="RectangleF"/> with all member data left uninitialized.
    /// </summary>
    public static readonly RectangleF Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleF"/> class.
    /// </summary>
    /// <param name="location">The coordinates of the upper-left corner of this rectangle.</param>
    /// <param name="size">The size of this rectangle.</param>
    public RectangleF(PointF location, SizeF size)
        : this(location.X, location.Y, size.Width, size.Height)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleF"/> class.
    /// </summary>
    /// <param name="rectangle">A rectangle whose coordinates and dimensions will be used.</param>
    public RectangleF(Rectangle rectangle) 
        : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleF"/> class.
    /// </summary>
    /// <param name="x">The x-coordinate of the upper-left corner of this rectangle.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of this rectangle.</param>
    /// <param name="width">The width of this rectangle.</param>
    /// <param name="height">The height of this rectangle.</param>
    public RectangleF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the x-coordinate of the upper-left corner of this rectangle.
    /// </summary>
    public float X
    { get; }

    /// <summary>
    /// Gets the y-coordinate of the upper-left corner of this rectangle.
    /// </summary>
    public float Y
    { get; }

    /// <summary>
    /// Gets the width of this rectangle.
    /// </summary>
    public float Width
    { get; }

    /// <summary>
    /// Gets the height of this rectangle.
    /// </summary>
    public float Height
    { get; }

    /// <summary>
    /// Gets the x-coordinate of the left edge of this rectangle.
    /// </summary>
    public float Left
        => X;

    /// <summary>
    /// Gets the y-coordinate of the top edge of this rectangle.
    /// </summary>
    public float Top
        => Y;

    /// <summary>
    /// Gets the x-coordinate of the right edge of this rectangle.
    /// </summary>
    public float Right
        => X + Width;

    /// <summary>
    /// Gets the y-coordinate of the bottom edge of this rectangle.
    /// </summary>
    public float Bottom
        => Y + Height;

    /// <inheritdoc/>
    public PointF Center
        => new(X + Width / 2f, Y + Height / 2f);

    /// <summary>
    /// Gets the coordinates of the upper-left corner of this rectangle.
    /// </summary>
    public PointF Location
        => new(X, Y);

    /// <summary>
    /// Gets the size of this rectangle.
    /// </summary>
    public SizeF Size
        => new(Width, Height);

    /// <summary>
    /// Gets a value indicating whether this rectangle is <see cref="Empty"/>.
    /// </summary>
    /// <remarks>
    /// The .NET runtime employs an inconsistently followed convention as far as <c>IsEmpty</c>-like properties
    /// for value types are concerned. Within Bad Echo frameworks, a value is considered empty if equal to one
    /// with all of its member data left uninitialized.
    /// </remarks>
    public bool IsEmpty
        => Equals(Empty);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Rectangle"/> value to a <see cref="RectangleF"/> value.
    /// </summary>
    /// <param name="rectangle">The rectangle to convert.</param> 
    public static implicit operator RectangleF(Rectangle rectangle)
        => FromRectangle(rectangle);

    /// <summary>
    /// Determines whether two <see cref="RectangleF"/> values have the same location and size.
    /// </summary>
    /// <param name="left">The first rectangle to compare.</param>
    /// <param name="right">The second rectangle to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same rectangular region as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(RectangleF left, RectangleF right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="RectangleF"/> values have a different location or size.
    /// </summary>
    /// <param name="left">The first rectangle to compare.</param>
    /// <param name="right">The second rectangle to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same rectangular region as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(RectangleF left, RectangleF right)
        => !left.Equals(right);

    /// <summary>
    /// Converts the specified <see cref="Rectangle"/> value to an equivalent <see cref="RectangleF"/> value.
    /// </summary>
    /// <param name="rectangle">The rectangle to convert.</param>
    /// <returns>A <see cref="RectangleF"/> value equivalent to <c>rectangle</c>.</returns>
    public static RectangleF FromRectangle(Rectangle rectangle)
        => new(rectangle);

    /// <summary>
    /// Determines whether two <see cref="RectangleF"/> values have the same location and size. 
    /// </summary>
    /// <param name="first">The first rectangle to compare.</param>
    /// <param name="second">The second rectangle to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same rectangular region as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(RectangleF first, RectangleF second)
        => first.Equals(second);

    /// <inheritdoc />
    public override bool Equals(object? obj) 
        => obj is RectangleF other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => this.GetHashCode(X, Y, Width, Height);

    /// <inheritdoc/>
    public override string ToString()
        => $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";

    /// <inheritdoc />
    public bool Equals(RectangleF other)
        => X.ApproximatelyEquals(other.X)
            && Y.ApproximatelyEquals(other.Y)
            && Width.ApproximatelyEquals(other.Width)
            && Height.ApproximatelyEquals(other.Height);

    /// <summary>
    /// Determines if the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if <c>point</c> is contained within this rectangle; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// Rectangles are endpoint exclusive. If a point lies on the right or bottom edge, it's considered to be
    /// outside of the rectangle.
    ///</para>
    /// <para>
    /// For example, a rectangle whose left corner is at x:0 and has a width of four will extend from the upper
    /// left corner of x:0 all the way to the upper right corner of x:3. Therefore, even though the width is four,
    /// a coordinate point of x:4 is actually outside of the rectangle.
    /// </para>
    /// </remarks>
    public bool Contains(PointF point)
        => X <= point.X && point.X < Right && Y <= point.Y && point.Y < Bottom;

    /// <summary>
    /// Determines if the specified rectangle is wholly contained within this rectangle.
    /// </summary>
    /// <param name="other">The rectangle to check.</param>
    /// <returns>True if <c>other</c> is wholly contained within this rectangle; otherwise, false.</returns>
    public bool Contains(RectangleF other)
        => X <= other.X && other.Right <= Right && Y <= other.Y && other.Bottom <= Bottom;

    /// <summary>
    /// Determines if the specified shape is wholly contained within this shape.
    /// </summary>
    /// <param name="other">The shape to check.</param>
    /// <returns>True if <c>other</c> is wholly contained within this rectangle; otherwise, false.</returns>
    public bool Contains(IShape other)
    {
        Require.NotNull(other, nameof(other));

        return other.Center.X + other.Width / 2 <= Right
            && other.Center.X - other.Width / 2 >= X
            && other.Center.Y + other.Height / 2 <= Bottom
            && other.Center.Y - other.Height / 2 >= Y;
    }

    /// <inheritdoc/>
    public PointF GetPointClosestTo(PointF point)
    {
        PointF upperLeft = Location;
        PointF bottomRight = new(Right, Bottom);

        float closestX = point.X;
        float closestY = point.Y;
        
        // Since our rectangles are axis-aligned, we can just clamp to the nearest point in the rectangle.
        if (closestX < upperLeft.X)
            closestX = upperLeft.X;
        else if (closestX > bottomRight.X)
            closestX = bottomRight.X;
        
        if (closestY < upperLeft.Y)
            closestY = upperLeft.Y;
        else if (closestY > bottomRight.Y)
            closestY = bottomRight.Y;
        
        return new PointF(closestX, closestY);
    }

    /// <summary>
    /// Creates a copy of this rectangle enlarged by the specified amount.
    /// </summary>
    /// <param name="size">The amount to inflate the rectangle.</param>
    /// <returns>The enlarged <see cref="RectangleF"/> value.</returns>
    public RectangleF Inflate(SizeF size)
        => new(X - size.Width, Y - size.Height, Width + size.Width * 2, Height + size.Height * 2);

    /// <summary>
    /// Creates a rectangle representing the intersection of this and the specified rectangle.
    /// </summary>
    /// <param name="other">The rectangle to intersect.</param>
    /// <returns>
    /// A <see cref="RectangleF"/> value which represents the overlapped area of this and <c>other</c>.
    /// </returns>
    public RectangleF Intersect(RectangleF other)
    {
        var x = Math.Max(X, other.X);
        var y = Math.Max(Y, other.Y);
        var right = Math.Min(Right, other.Right);
        var bottom = Math.Min(Bottom, other.Bottom);
        
        return right >= x && bottom >= y ? new RectangleF(x, y, right - x, bottom - y) : Empty;
    }

    /// <summary>
    /// Determines if this rectangle intersects with the specified rectangle.
    /// </summary>
    /// <param name="other">The rectangle to check.</param>
    /// <returns>True if <c>other</c> intersects with this; otherwise, false.</returns>
    public bool Intersects(RectangleF other)
        => other.X < Right && X < other.Right && other.Y < Bottom && Y < other.Bottom;

    /// <summary>
    /// Creates a copy of this rectangle with its position adjusted by the specified amount.
    /// </summary>
    /// <param name="position">The amount to offset the location.</param>
    /// <returns>The location-adjusted<see cref="RectangleF"/> value.</returns>
    public RectangleF Offset(PointF position)
        => new(X + position.X, Y + position.Y, Width, Height);

    /// <summary>
    /// Creates a rectangle representing the union between this and the specified rectangle.
    /// </summary>
    /// <param name="other">The rectangle to create a union with.</param>
    /// <returns>
    /// A <see cref="RectangleF"/> value representing the union between this and <c>other</c>.
    /// </returns>
    public RectangleF Union(RectangleF other)
    {
        var x = Math.Min(X, other.X);
        var y = Math.Min(Y, other.Y);
        var right = Math.Max(Right, other.Right);
        var bottom = Math.Max(Bottom, other.Bottom);

        return new RectangleF(x, y, right - x, bottom - y);
    }
}
