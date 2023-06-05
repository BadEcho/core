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

namespace BadEcho.Game.UI;

/// <summary>
/// Represents the measure of space in a <see cref="Grid"/> in one direction.
/// </summary>
/// <suppressions>
/// ReSharper disable UnassignedReadonlyField
/// </suppressions>
public readonly struct GridDimension : IEquatable<GridDimension>
{
    /// <summary>
    /// Represents an empty <see cref="GridDimension"/> with all member data left uninitialized.
    /// </summary>
    public static readonly GridDimension Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridDimension"/> class.
    /// </summary>
    /// <param name="pixels">The measurement value in pixels.</param>
    public GridDimension(float pixels)
        : this(pixels, GridDimensionUnit.Absolute)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridDimension"/> class.
    /// </summary>
    /// <param name="value">The measurement value.</param>
    /// <param name="unit">The unit of measurement that the provided value uses.</param>
    public GridDimension(float value, GridDimensionUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Gets the unit of measurement that <see cref="Value"/> uses.
    /// </summary>
    public GridDimensionUnit Unit
    { get; }

    /// <summary>
    /// Gets the value for this <see cref="GridDimension"/>.
    /// </summary>
    public float Value
    { get; }

    /// <summary>
    /// Gets a value indicating whether this grid dimension is <see cref="Empty"/>.
    /// </summary>
    /// <remarks>
    /// The .NET runtime employs an inconsistently followed convention as far as <c>IsEmpty</c>-like properties
    /// for value types are concerned. Within Bad Echo frameworks, a value is considered empty if equal to one
    /// with all of its member data left uninitialized.
    /// </remarks>
    public bool IsEmpty
        => Equals(Empty);

    /// <summary>
    /// Determines whether two <see cref="GridDimension"/> values have the same unit of measurement and value.
    /// </summary>
    /// <param name="left">The first grid dimension to compare.</param>
    /// <param name="right">The second grid dimension to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same measurement as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(GridDimension left, GridDimension right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="GridDimension"/> values have a different unit of measurement or value.
    /// </summary>
    /// <param name="left">The first grid dimension to compare.</param>
    /// <param name="right">The second grid dimension to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same measurement as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(GridDimension left, GridDimension right)
        => !left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="GridDimension"/> values have the same unit of measurement and value.
    /// </summary>
    /// <param name="first">The first grid dimension to compare.</param>
    /// <param name="second">The second grid dimension to compare.</param>
    /// <returns>
    /// True if <c>first</c> represents the same rectangular region as <c>second</c>; otherwise, false.
    /// </returns>
    public static bool Equals(GridDimension first, GridDimension second)
        => first.Equals(second);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is GridDimension other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => this.GetHashCode(Unit, Value);
    
    /// <inheritdoc />
    public bool Equals(GridDimension other)
        => Unit == other.Unit && Value.ApproximatelyEquals(other.Value);
}
