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

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a value converter that converts provided <see cref="double"/> values to <see cref="Thickness"/> values which
/// use the provided <see cref="double"/> value for one or more of its edges.
/// </summary>
[ValueConversion(typeof(double), typeof(Thickness))]
public sealed class DoubleToThicknessConverter : ReversibleValueConverter<double, Thickness>
{
    /// <summary>
    /// Gets or sets the set of specific edges to set when outputting <see cref="Thickness"/> values, as well as the edge read
    /// from when the conversion direction is reversed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the conversion direction is reversed and more than one <see cref="Edges"/> value is set for the scope, then (as only one value can
    /// be returned ), preference is given to edges in the following order: Bottom -> Left -> Right -> Top.
    /// </para>
    /// <para>
    /// Failing to set the scope results in all output <see cref="Thickness"/> values ending up being composed of purely default values, as
    /// well as output <see cref="double"/> values defaulting to the input thickness's <see cref="Thickness.Top"/> property value.
    /// </para>
    /// </remarks>
    public Edges Scope
    { get; set; }

    /// <inheritdoc/>
    protected override Thickness Convert(double value, object? parameter, CultureInfo culture)
    {
        // Wow, an actual mutable value type.
        var thickness = new Thickness();
            
        if (Scope.HasFlag(Edges.Bottom)) 
            thickness.Bottom = value;

        if (Scope.HasFlag(Edges.Left))
            thickness.Left = value;

        if (Scope.HasFlag(Edges.Right))
            thickness.Right = value;

        if (Scope.HasFlag(Edges.Top))
            thickness.Top = value;

        return thickness;
    }

    /// <inheritdoc/>
    protected override double ConvertBack(Thickness value, object? parameter, CultureInfo culture)
    {
        return Scope.HasFlag(Edges.Bottom)
            ? value.Bottom
            : Scope.HasFlag(Edges.Left)
                ? value.Left
                : Scope.HasFlag(Edges.Right) ? value.Right : value.Top;
    }
}