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
    /// Initializes a new instance of the <see cref="Thickness"/> class.
    /// </summary>
    /// <param name="left">The width of the left side of the bounding rectangle.</param>
    /// <param name="top">The width of the top side of the bounding rectangle.</param>
    /// <param name="right">The width of the right side of the bounding rectangle.</param>
    /// <param name="bottom">The width of the bottom side of the bounding rectangle.</param>
    public Thickness(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
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
}
