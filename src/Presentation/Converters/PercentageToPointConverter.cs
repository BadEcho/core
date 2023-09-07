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

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a value converter that converts provided percentage values to a corresponding
/// x- and y-coordinate pair in two-dimensional space.
/// </summary>
public sealed class PercentageToPointConverter : ReversibleValueConverter<double,Point>
{
    /// <summary>
    /// Gets or sets the relationship between incoming percentage values and the x-coordinates
    /// they are to be converted into.
    /// </summary>
    /// <remarks>
    /// A ratio of 1 to 1 (which the default value of 1.0 represents) would result in an incoming
    /// percentage value of 50% (0.5) being converted to a point where the x-coordinate is set to 0.5.
    /// </remarks>
    public double PercentageToXRatio
    { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the relationship between incoming percentage values and the y-coordinates they
    /// are to be converted into.
    /// </summary>
    /// <remarks>
    /// A ratio of 1 to 1 (which the default value of 1.0 represents) would result in an incoming
    /// percentage value of 50% (0.5) being converted to a point where the y-coordinate is set to 0.5.
    /// </remarks>
    public double PercentageToYRatio
    { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the point value cascade amount that occurs when incoming percentage values transition
    /// from whole amounts to proper fractional values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The relationship between incoming percentage values and the converted point values essentially linear,
    /// except when the input percentages begin to fall underneath whole amounts and become fractional, which
    /// would be anything less than 100% (0.1). When this occurs, a cascade can be applied which will apply an
    /// immediate reductionary adjustment equal to this property's value.
    /// </para>
    /// <para>
    /// In the opposite case, where incoming percentage values cross from fractional to whole, a reverse cascade
    /// is applied. The default amount this is set to is 0, which means, by default, no cascading occurs.
    /// </para>
    /// <para>
    /// An input value is considered "fractional" if it is a proper fraction, improper fractions are excluded
    /// from any type of cascading effect, as they are essentially "whole amounts and then some". Yes,
    /// excellent use of proper nomenclature, I know.
    /// </para>
    /// </remarks>
    public double FractionalCascadeAmount
    { get; set; }

    /// <inheritdoc/>
    protected override Point Convert(double value, object? parameter, CultureInfo culture)
    {
        if (value >= 1.0)
            value += FractionalCascadeAmount;

        double xCoordinate = value * PercentageToXRatio;
        double yCoordinate = value * PercentageToYRatio;

        return new Point(xCoordinate, yCoordinate);
    }

    /// <inheritdoc/>
    protected override double ConvertBack(Point value, object? parameter, CultureInfo culture)
    {   
        // Not going to bother too much with intense validation here, as we don't want to ever throw errors
        // in an IValueConverter. We assume the point value was created by this converter, and convert the
        // value back based on the looking at the first coordinate point we come across.
        double percentage = value.X / PercentageToXRatio;

        if (percentage >= 1.0 + FractionalCascadeAmount)
            percentage -= FractionalCascadeAmount;

        return percentage;
    }
}