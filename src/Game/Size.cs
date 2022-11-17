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
using BadEcho.Extensions;

namespace BadEcho.Game;

/// <summary>
/// Represents the size of a rectangular region by its width and height.
/// </summary>
public readonly struct Size : IEquatable<Size>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Size"/> class.
    /// </summary>
    /// <param name="point">A 2D-point value whose coordinates will be used as the width and height.</param>
    public Size(Point point)
        : this(point.X, point.Y)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Size"/> class.
    /// </summary>
    /// <param name="width">The horizontal component of this size.</param>
    /// <param name="height">The vertical component of this size.</param>
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets a value that represents a static empty <see cref="Size"/>.
    /// </summary>
    public static Size Empty
        => new ();

    /// <summary>
    /// Gets the horizontal component of this size.
    /// </summary>
    public int Width
    { get; }

    /// <summary>
    /// Gets the vertical component of this size.
    /// </summary>
    public int Height
    { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is <see cref="Empty"/>.
    /// </summary>
    public bool IsEmpty
        => Width == 0 && Height == 0;

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Point"/> value to a <see cref="Size"/> value.
    /// </summary>
    /// <param name="point">The 2D-point value to convert.</param>
    public static implicit operator Size(Point point)
        => FromPoint(point);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Size"/> value to a <see cref="Point"/> value.
    /// </summary>
    /// <param name="size">The size value to convert.</param>
    public static implicit operator Point(Size size)
        => ToPoint(size);

    /// <summary>
    /// Determines whether two <see cref="Size"/> values have the same width and height.
    /// </summary>
    /// <param name="left">The first size to compare.</param>
    /// <param name="right">The second size to compare.</param>
    /// <returns>True if <c>left</c> represents the same rectangular region size as <c>right</c>; otherwise, false.</returns>
    public static bool operator ==(Size left, Size right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Size"/> values differ in width or height.
    /// </summary>
    /// <param name="left">The first size to compare.</param>
    /// <param name="right">The second size to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same rectangular region size as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(Size left, Size right)
        => !left.Equals(right);

    /// <summary>
    /// Adds two <see cref="Size"/> values together via vector addition to compute their sum.
    /// </summary>
    /// <param name="left">The size to which <c>right</c> is added.</param>
    /// <param name="right">The size which is added to <c>left</c>.</param>
    /// <returns>The vector sum of <c>left</c> and <c>right</c>.</returns>
    public static Size operator +(Size left, Size right)
        => left.Add(right);

    /// <summary>
    /// Subtracts two <see cref="Size"/> values via vector subtraction to compute their difference.
    /// </summary>
    /// <param name="left">The size from which <c>right</c> is subtracted.</param>
    /// <param name="right">The size which is subtracted from <c>left</c>.</param>
    /// <returns>The vector difference of <c>right</c> from <c>left</c>.</returns>
    public static Size operator -(Size left, Size right)
        => left.Subtract(right);

    /// <summary>
    /// Multiplies the <see cref="Size"/> value by the integer value via scalar multiplication to compute
    /// their product.
    /// </summary>
    /// <param name="left">The size which <c>right</c> multiplies.</param>
    /// <param name="right">The integer which multiplies <c>left</c>.</param>
    /// <returns>The product of <c>left</c> multiplied by <c>right</c>.</returns>
    public static Size operator *(Size left, int right)
        => left.Multiply(right);

    /// <summary>
    /// Multiplies the <see cref="Size"/> value by the integer value via scalar multiplication to compute
    /// their product.
    /// </summary>
    /// <param name="left">The integer which multiplies <c>right</c>.</param>
    /// <param name="right">The size which <c>left</c> multiplies.</param>
    /// <returns>The product of <c>right</c> multiplied by <c>left</c>.</returns>
    public static Size operator *(int left, Size right)
        => right.Multiply(left);

    /// <summary>
    /// Multiplies two <see cref="Size"/> values together via multiplication of corresponding directional
    /// components to compute their product.
    /// </summary>
    /// <param name="left">The size which <c>right</c> multiplies.</param>
    /// <param name="right">The size which multiplies <c>left</c>.</param>
    /// <returns>The product of this size multiplied by <c>size</c>.</returns>
    public static Size operator *(Size left, Size right)
        => left.Multiply(right);

    /// <summary>
    /// Divides the <see cref="Size"/> value by the integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="left">The size which <c>right</c> divides.</param>
    /// <param name="right">The integer which divides <c>left</c>.</param>
    /// <returns>The quotient of <c>left</c> divided by <c>right</c>.</returns>
    public static Size operator /(Size left, int right)
        => left.Divide(right);

    /// <summary>
    /// Divides one <see cref="Size"/> value by another via division of corresponding directional components
    /// to compute their quotient.
    /// </summary>
    /// <param name="left">The size which <c>right</c> divides.</param>
    /// <param name="right">The size which divides <c>left</c>.</param>
    /// <returns>The quotient of <c>left</c> divided by <c>right</c>.</returns>
    public static Size operator /(Size left, Size right)
        => left.Divide(right);

    /// <summary>
    /// Converts the specified <see cref="Point"/> value to an equivalent <see cref="Size"/> value.
    /// </summary>
    /// <param name="point">The 2D-point value to convert.</param>
    /// <returns>A <see cref="Size"/> value equivalent to <c>point</c>.</returns>
    public static Size FromPoint(Point point)
        => new(point);

    /// <summary>
    /// Converts the specified <see cref="Size"/> value to an equivalent <see cref="Point"/> value.
    /// </summary>
    /// <param name="size">The size value to convert.</param>
    /// <returns>A <see cref="Point"/> value equivalent to <c>size</c>.</returns>
    public static Point ToPoint(Size size)
        => new(size.Width, size.Height);

    /// <summary>
    /// Determines whether two <see cref="Size"/> values have the same width and height.
    /// </summary>
    /// <param name="first">The first size to compare.</param>
    /// <param name="second">The second size to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same rectangular region size as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(Size first, Size second)
        => first.Equals(second);

    /// <inheritdoc />
    public override bool Equals(object? obj) 
        => obj is Size other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => this.GetHashCode(Width, Height);

    /// <inheritdoc/>
    public override string ToString()
        => $"Width: {Width}, Height: {Height}";

    /// <inheritdoc />
    public bool Equals(Size other)
        => Width == other.Width && Height == other.Height;

    /// <summary>
    /// Adds this size with the specified <see cref="Size"/> value via vector addition to compute their sum.
    /// </summary>
    /// <param name="other">The size to add.</param>
    /// <returns>The vector sum of this size and <c>other</c>.</returns>
    public Size Add(Size other)
        => new(unchecked(Width + other.Width), unchecked(Height + other.Height));

    /// <summary>
    /// Subtracts the specified <see cref="Size"/> value from this size via vector subtraction to compute their
    /// difference.
    /// </summary>
    /// <param name="other">The size which is subtracted.</param>
    /// <returns>The vector difference of <c>other</c> from this size.</returns>
    public Size Subtract(Size other)
        => new(unchecked(Width - other.Width), unchecked(Height - other.Height));

    /// <summary>
    /// Multiplies this size by the specified integer value via scalar multiplication to compute their product.
    /// </summary>
    /// <param name="multiplier">The integer to be multiplied by.</param>
    /// <returns>The product of this size multiplied by <c>multiplier</c>.</returns>
    public Size Multiply(int multiplier)
        => new(unchecked(Width * multiplier), unchecked(Height * multiplier));

    /// <summary>
    /// Multiplies this size by the specified <see cref="Size"/> value via multiplication of corresponding directional
    /// components to compute their product.
    /// </summary>
    /// <param name="other">The size to be multiplied by.</param>
    /// <returns>The product of this size multiplied by <c>size</c>.</returns>
    public Size Multiply(Size other)
        => new(unchecked(Width * other.Width), unchecked(Height * other.Height));

    /// <summary>
    /// Divides this size by the specified integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="divisor">The integer to be divided by.</param>
    /// <returns>The quotient of this size divided by <c>divisor</c>.</returns>
    public Size Divide(int divisor)
        => new(Width / divisor, Height / divisor);

    /// <summary>
    /// Divides this size by the specified <see cref="Size"/> value via division of corresponding directional components
    /// to compute their quotient.
    /// </summary>
    /// <param name="other">The size to be divided by.</param>
    /// <returns>The quotient of this size divided by <c>other</c>.</returns>
    public Size Divide(Size other)
        => new(Width / other.Width, Height / other.Height);
}
