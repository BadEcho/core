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
using BadEcho.Extensions;

namespace BadEcho.Game;

/// <summary>
/// Represents the size of a geometric region defined by an ordered pair of floating-point numbers for the width and height.
/// </summary>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct SizeF : IEquatable<SizeF>
{
    /// <summary>
    /// Represents an empty <see cref="SizeF"/> with all member data left uninitialized.
    /// </summary>
    public static readonly SizeF Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> class.
    /// </summary>
    /// <param name="point">A point whose coordinates will be used as the width and height.</param>
    public SizeF(Point point)
        : this(point.X, point.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> class.
    /// </summary>
    /// <param name="point">A point whose coordinates will be used as the width and height.</param>
    public SizeF(PointF point)
        : this(point.X, point.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> class.
    /// </summary>
    /// <param name="vector">A vector whose two floating-point values will be used as the width and height.</param>
    public SizeF(Vector2 vector)
        : this(vector.X, vector.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> class.
    /// </summary>
    /// <param name="size">A size who width and height will be used.</param>
    public SizeF(Size size)
        : this(size.Width, size.Height)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> class.
    /// </summary>
    /// <param name="width">The horizontal component of this size.</param>
    /// <param name="height">The vertical component of this size.</param>
    public SizeF(float width, float height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the horizontal component of this size.
    /// </summary>
    public float Width
    { get; }

    /// <summary>
    /// Gets the vertical component of this size.
    /// </summary>
    public float Height
    { get; }

    /// <summary>
    /// Gets a value indicating whether this size is <see cref="Empty"/>.
    /// </summary>
    /// <remarks>
    /// The .NET runtime employs an inconsistently followed convention as far as <c>IsEmpty</c>-like properties
    /// for value types are concerned. Within Bad Echo frameworks, a value is considered empty if equal to one
    /// with all of its member data left uninitialized.
    /// </remarks>
    public bool IsEmpty
        => Equals(Empty);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Point"/> value to a <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    public static implicit operator SizeF(Point point)
        => FromPoint(point);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="PointF"/> value to a <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param> 
    public static implicit operator SizeF(PointF point)
        => FromPointF(point);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Vector2"/> value to a <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    public static implicit operator SizeF(Vector2 vector)
        => FromVector2(vector);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Size"/> value to a <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    public static implicit operator SizeF(Size size)
        => FromSize(size);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="SizeF"/> value to a <see cref="PointF"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    public static implicit operator PointF(SizeF size)
        => ToPointF(size);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="SizeF"/> value to a <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    public static implicit operator Vector2(SizeF size)
        => ToVector2(size);

    /// <summary>
    /// Defines an explicit conversion of a <see cref="SizeF"/> value to a <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    public static explicit operator Size(SizeF size)
        => ToSize(size);

    /// <summary>
    /// Determines whether two <see cref="SizeF"/> values have the same width and height.
    /// </summary>
    /// <param name="left">The first size to compare.</param>
    /// <param name="right">The second size to compare.</param>
    /// <returns>True if <c>left</c> represents the same geometric region size as <c>right</c>; otherwise, false.</returns>
    public static bool operator ==(SizeF left, SizeF right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="SizeF"/> values differ in width or height.
    /// </summary>
    /// <param name="left">The first size to compare.</param>
    /// <param name="right">The second size to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same geometric region size as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(SizeF left, SizeF right)
        => !left.Equals(right);

    /// <summary>
    /// Adds two <see cref="SizeF"/> values together via vector addition to compute their sum.
    /// </summary>
    /// <param name="left">The size to which <c>right</c> is added.</param>
    /// <param name="right">The size which is added to <c>left</c>.</param>
    /// <returns>The vector sum of <c>left</c> and <c>right</c>.</returns>
    public static SizeF operator +(SizeF left, SizeF right)
        => left.Add(right);

    /// <summary>
    /// Subtracts two <see cref="SizeF"/> values via vector subtraction to compute their difference.
    /// </summary>
    /// <param name="left">The size from which <c>right</c> is subtracted.</param>
    /// <param name="right">The size which is subtracted from <c>left</c>.</param>
    /// <returns>The vector difference of <c>right</c> from <c>left</c>.</returns>
    public static SizeF operator -(SizeF left, SizeF right)
        => left.Subtract(right);

    /// <summary>
    /// Multiplies the <see cref="SizeF"/> value by the integer value via scalar multiplication to compute
    /// their product.
    /// </summary>
    /// <param name="left">The size which <c>right</c> multiplies.</param>
    /// <param name="right">The integer which multiplies <c>left</c>.</param>
    /// <returns>The product of <c>left</c> multiplied by <c>right</c>.</returns>
    public static SizeF operator *(SizeF left, int right)
        => left.Multiply(right);

    /// <summary>
    /// Multiplies the <see cref="SizeF"/> value by the integer value via scalar multiplication to compute
    /// their product.
    /// </summary>
    /// <param name="left">The integer which multiplies <c>right</c>.</param>
    /// <param name="right">The size which <c>left</c> multiplies.</param>
    /// <returns>The product of <c>right</c> multiplied by <c>left</c>.</returns>
    public static SizeF operator *(int left, SizeF right)
        => right.Multiply(left);

    /// <summary>
    /// Divides the <see cref="SizeF"/> value by the integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="left">The size which <c>right</c> divides.</param>
    /// <param name="right">The integer which divides <c>left</c>.</param>
    /// <returns>The quotient of <c>left</c> divided by <c>right</c>.</returns>
    public static SizeF operator /(SizeF left, int right)
        => left.Divide(right);

    /// <summary>
    /// Converts the specified <see cref="Point"/> value to an equivalent <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <returns>A <see cref="SizeF"/> value equivalent to <c>point</c>.</returns>
    public static SizeF FromPoint(Point point)
        => new(point);

    /// <summary>
    /// Converts the specified <see cref="PointF"/> value to an equivalent <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <returns>A <see cref="SizeF"/> value equivalent to <c>point</c>.</returns>
    public static SizeF FromPointF(PointF point)
        => new(point);

    /// <summary>
    /// Converts the specified <see cref="Vector2"/> value to an equivalent <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A <see cref="SizeF"/> value equivalent to <c>vector</c>.</returns>
    public static SizeF FromVector2(Vector2 vector)
        => new(vector);

    /// <summary>
    /// Converts the specified <see cref="Size"/> value to an equivalent <see cref="SizeF"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <returns>A <see cref="SizeF"/> value equivalent to <c>size</c>.</returns>
    public static SizeF FromSize(Size size)
        => new(size);

    /// <summary>
    /// Converts the specified <see cref="SizeF"/> value to an equivalent <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <returns>A <see cref="Size"/> value equivalent to <c>size</c>.</returns>
    public static Size ToSize(SizeF size)
        => new((int) Math.Round(size.Width, 0), (int) Math.Round(size.Height, 0));

    /// <summary>
    /// Converts the specified <see cref="SizeF"/> value to an equivalent <see cref="PointF"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <returns>A <see cref="PointF"/> value equivalent to <c>size</c>.</returns>
    public static PointF ToPointF(SizeF size)
        => new (size.Width, size.Height);

    /// <summary>
    /// Converts the specified <see cref="SizeF"/> value to an equivalent <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <returns>A <see cref="Vector2"/> value equivalent to <c>size</c>.</returns>
    public static Vector2 ToVector2(SizeF size) 
        => new(size.Width, size.Height);

    /// <summary>
    /// Determines whether two <see cref="SizeF"/> values have the same width and height.
    /// </summary>
    /// <param name="first">The first size to compare.</param>
    /// <param name="second">The second size to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same geometric region size as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(SizeF first, SizeF second)
        => first.Equals(second);

    /// <inheritdoc/> 
    public override bool Equals(object? obj) 
        => obj is SizeF other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Width, Height);

    /// <inheritdoc/>
    public override string ToString()
        => $"Width: {Width}, Height: {Height}";

    /// <inheritdoc/>
    public bool Equals(SizeF other)
        => Width.ApproximatelyEquals(other.Width) && Height.ApproximatelyEquals(other.Height);

    /// <summary>
    /// Adds this size with the specified <see cref="SizeF"/> value via vector addition to compute their sum.
    /// </summary>
    /// <param name="other">The size to add.</param>
    /// <returns>The vector sum of this size and <c>other</c>.</returns>
    public SizeF Add(SizeF other)
        => new(Width + other.Width, Height + other.Height);

    /// <summary>
    /// Subtracts the specified <see cref="SizeF"/> value from this size via vector subtraction to compute their
    /// difference.
    /// </summary>
    /// <param name="other">The size which is subtracted.</param>
    /// <returns>The vector difference of <c>other</c> from this size.</returns>
    public SizeF Subtract(SizeF other)
        => new(Width - other.Width, Height - other.Height);

    /// <summary>
    /// Multiplies this size by the specified integer value via scalar multiplication to compute their product.
    /// </summary>
    /// <param name="multiplier">The integer to be multiplied by.</param>
    /// <returns>The product of this size multiplied by <c>multiplier</c>.</returns>
    public SizeF Multiply(float multiplier)
        => new(Width * multiplier, Height * multiplier);

    /// <summary>
    /// Divides this size by the specified integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="divisor">The integer to be divided by.</param>
    /// <returns>The quotient of this size divided by <c>divisor</c>.</returns>
    public SizeF Divide(float divisor)
        => new(Width / divisor, Height / divisor);
}
