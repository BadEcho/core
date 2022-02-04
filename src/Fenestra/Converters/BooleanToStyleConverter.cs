//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Fenestra.Converters;

/// <summary>
/// Provides a value converter that produces <see cref="Style"/> objects equivalent to the Boolean values provided, and vice
/// versa.
/// </summary>
[ValueConversion(typeof(bool), typeof(Style))]
public sealed class BooleanToStyleConverter : ValueConverter<bool,Style?>
{
    /// <summary>
    /// Gets or sets the <see cref="Style"/> that <c>true</c> input values are converted into.
    /// </summary>
    public Style? StyleWhenTrue
    { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> that <c>false</c> input values are converted into.
    /// </summary>
    public Style? StyleWhenFalse
    {  get; set; }

    /// <inheritdoc/>
    protected override Style? Convert(bool value, object parameter, CultureInfo culture)
        => value ? StyleWhenTrue : StyleWhenFalse;

    /// <inheritdoc/>
    protected override bool ConvertBack(Style? value, object parameter, CultureInfo culture)
        => StyleWhenTrue == value;
}