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

using BadEcho.Extensions;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Represents the thickness of a frame around a rectangle.
/// </summary>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct Thickness : IEquatable<Thickness>
{
    /// <summary>
    /// Represents an empty <see cref="Thickness"/> with all member data left uninitialized.
    /// </summary>
    public static readonly Thickness Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Thickness"/> class.;
    /// </summary>
    /// <param name="left">The width of the left side of the bounding rectangle.</param>
    /// <param name="top">The height of the top side of the bounding rectangle.</param>
    /// <param name="right">The width of the right side of the bounding rectangle.</param>
    /// <param name="bottom">The height of the bottom side of the bounding rectangle.</param>
    public Thickness(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Thickness"/> class.
    /// </summary>
    /// <param name="uniformLength">The uniform length applied to all four sides of the bounding rectangle.</param>
    public Thickness(int uniformLength)
    {
        Left = uniformLength;
        Top = uniformLength;
        Right = uniformLength;
        Bottom = uniformLength;
    }

    /// <summary>
    /// Gets the width of the left side of the bounding rectangle.
    /// </summary>
    public int Left
    { get; }

    /// <summary>
    /// Gets the height of the top side of the bounding rectangle.
    /// </summary>
    public int Top
    { get; }

    /// <summary>
    /// Gets the width of the right side of the bounding rectangle.
    /// </summary>
    public int Right
    { get; }

    /// <summary>
    /// Gets the height of the bottom side of the bounding rectangle.
    /// </summary>
    public int Bottom
    { get; }

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
    /// Determines whether two <see cref="Thickness"/> values have the same dimensions.
    /// </summary>
    /// <param name="left">The first thickness to compare.</param>
    /// <param name="right">The second thickness to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same thickness as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(Thickness left, Thickness right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Thickness"/> values have different dimensions.
    /// </summary>
    /// <param name="left">The first thickness to compare.</param>
    /// <param name="right">The second thickness to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same thickness as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(Thickness left, Thickness right)
        => !left.Equals(right);

    /// <summary>
    /// Adds two <see cref="Thickness"/> values together via vector addition to compute their sum.
    /// </summary>
    /// <param name="left">The thickness to which <c>right</c> is added.</param>
    /// <param name="right">The thickness which is added to <c>left</c>.</param>
    /// <returns>The sum of <c>left</c> and <c>right</c>.</returns>
    public static Thickness operator +(Thickness left, Thickness right)
        => left.Add(right);

    /// <summary>
    /// Subtracts two <see cref="Thickness"/> values via vector subtraction to compute their difference.
    /// </summary>
    /// <param name="left">The thickness from which <c>right</c> is subtracted.</param>
    /// <param name="right">The thickness subtracted from <c>left</c>.</param>
    /// <returns>The difference of <c>right</c> from <c>left</c>.</returns>
    public static Thickness operator -(Thickness left, Thickness right)
        => left.Subtract(right);

    /// <summary>
    /// Multiplies the <see cref="Thickness"/> value by the integer value via scalar multiplication to compute their
    /// product.
    /// </summary>
    /// <param name="left">The thickness which <c>right</c> multiplies.</param>
    /// <param name="right">The integer which multiplies <c>left</c>.</param>
    /// <returns>The product of <c>left</c> multiplied by <c>right</c>.</returns>
    public static Thickness operator *(Thickness left, int right)
        => left.Multiply(right);

    /// <summary>
    /// Multiplies the <see cref="Thickness"/> value by the integer value via scalar multiplication to compute their
    /// product.
    /// </summary>
    /// <param name="left">The integer which multiplies <c>right</c>.</param>
    /// <param name="right">The thickness which <c>left</c> multiplies.</param>
    /// <returns>The product of <c>right</c> multiplied by <c>left</c>.</returns>
    public static Thickness operator *(int left, Thickness right)
        => right.Multiply(left);

    /// <summary>
    /// Multiplies two <see cref="Thickness"/> values together via multiplication of corresponding positional components
    /// to compute their product.
    /// </summary>
    /// <param name="left">The thickness which <c>right</c> multiplies.</param>
    /// <param name="right">The thickness which multiplies <c>left</c>.</param>
    /// <returns>The product of <c>left</c> multiplied by <c>right</c>.</returns>
    public static Thickness operator *(Thickness left, Thickness right)
        => left.Multiply(right);

    /// <summary>
    /// Divides the <see cref="Thickness"/> value by the integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="left">The thickness which <c>right</c> divides.</param>
    /// <param name="right">The integer which divides <c>left</c>.</param>
    /// <returns>The quotient of <c>left</c> divided by <c>right</c>.</returns>
    public static Thickness operator /(Thickness left, int right)
        => left.Divide(right);

    /// <summary>
    /// Divides one <see cref="Thickness"/> value by another via division of corresponding positional components to
    /// compute their quotient.
    /// </summary>
    /// <param name="left">The thickness which <c>right</c> divides.</param>
    /// <param name="right">The thickness which divides <c>left</c>.</param>
    /// <returns>The quotient of <c>left</c> divided by <c>>right</c>.</returns>
    public static Thickness operator /(Thickness left, Thickness right)
        => left.Divide(right);

    /// <summary>
    /// Determines whether two <see cref="Thickness"/> values have the same dimensions.
    /// </summary>
    /// <param name="first">The first thickness to compare.</param>
    /// <param name="second">The second thickness to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same thickness as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(Thickness first, Thickness second)
        => first.Equals(second);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Thickness other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Left, Top, Right, Bottom);

    /// <inheritdoc/>
    public override string ToString()
        => $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
    
    /// <inheritdoc/>
    public bool Equals(Thickness other)
        => Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;

    /// <summary>
    /// Applies this thickness as a margin, shrinking the effective size of the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to apply a margin to.</param>
    /// <returns>The margined <see cref="Rectangle"/> value.</returns>
    public Rectangle ApplyMargin(Rectangle rectangle)
        => new(rectangle.X + Left,
               rectangle.Y + Top,
               Math.Max(0, rectangle.Width - (Left + Right)),
               Math.Max(0, rectangle.Height - (Top + Bottom)));

    /// <summary>
    /// Applies this thickness as a margin, shrinking the specified size.
    /// </summary>
    /// <param name="size">The size to apply a margin to.</param>
    /// <returns>The margined <see cref="Size"/> value.</returns>
    public Size ApplyMargin(Size size)
        => new(Math.Max(0, size.Width - (Left + Right)), Math.Max(0, size.Height - (Top + Bottom)));

    /// <summary>
    /// Applies this thickness as a padding, enlarging the effective size of the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to pad.</param>
    /// <returns>The padded <see cref="Rectangle"/> value.</returns>
    public Rectangle ApplyPadding(Rectangle rectangle)
        => new(rectangle.X - Left,
               rectangle.Y - Top,
               Math.Max(0, rectangle.Width + Left + Right),
               Math.Max(0, rectangle.Height + Top + Bottom));

    /// <summary>
    /// Applies this thickness as a padding, enlarging the specified size.
    /// </summary>
    /// <param name="size">The size to pad.</param>
    /// <returns>The padded <see cref="Size"/> value.</returns>
    public Size ApplyPadding(Size size)
        => new(Math.Max(0, size.Width + Left + Right), Math.Max(0, size.Height + Top + Bottom));

    /// <summary>
    /// Adds this thickness to the specified <see cref="Thickness"/> value via vector addition to compute their sum.
    /// </summary>
    /// <param name="other">The thickness to add.</param>
    /// <returns>The sum of this size and <c>other</c>.</returns>
    public Thickness Add(Thickness other)
        => new(Left + other.Left, Top + other.Top, Right + other.Right, Bottom + other.Bottom);

    /// <summary>
    /// Subtracts the specified <see cref="Thickness"/> value from this thickness via vector subtraction to compute their
    /// difference.
    /// </summary>
    /// <param name="other">The thickness which is subtracted.</param>
    /// <returns>The difference of <c>other</c> from this thickness.</returns>
    public Thickness Subtract(Thickness other)
        => new(Left - other.Left, Top - other.Top, Right - other.Right, Bottom - other.Bottom);

    /// <summary>
    /// Multiplies this thickness by the specified integer value via scalar multiplication to compute their product.
    /// </summary>
    /// <param name="multiplier">The integer to be multiplied by.</param>
    /// <returns>The product of this thickness multiplied by <c>multiplier</c>.</returns>
    public Thickness Multiply(int multiplier)
        => new(Left * multiplier, Top * multiplier, Right * multiplier, Bottom * multiplier);

    /// <summary>
    /// Multiplies this thickness by the specified <see cref="Thickness"/> value via multiplication of corresponding
    /// positional components to compute their product.
    /// </summary>
    /// <param name="other">The thickness to be multiplied by.</param>
    /// <returns>The product of this thickness multiplied by <c>other</c>.</returns>
    public Thickness Multiply(Thickness other)
        => new(Left * other.Left, Top * other.Top, Right * other.Right, Bottom * other.Bottom);

    /// <summary>
    /// Divides this thickness by the specified integer value via scalar division to compute their quotient.
    /// </summary>
    /// <param name="divisor">The integer to be divided by.</param>
    /// <returns>The quotient of this thickness divided by <c>divisor</c>.</returns>
    public Thickness Divide(int divisor)
        => new(Left / divisor, Top / divisor, Right / divisor, Bottom / divisor);

    /// <summary>
    /// Divides this thickness by the specified <see cref="Thickness"/> value via division of corresponding positional
    /// components to compute their quotient.
    /// </summary>
    /// <param name="other">The thickness to be divided by.</param>
    /// <returns>The quotient of this thickness divided by <c>other</c>.</returns>
    public Thickness Divide(Thickness other)
        => new(Left / other.Left, Top / other.Top, Right / other.Right, Bottom / other.Bottom);
}
