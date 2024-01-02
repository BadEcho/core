//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
    protected override Style? Convert(bool value, object? parameter, CultureInfo culture)
        => value ? StyleWhenTrue : StyleWhenFalse;

    /// <inheritdoc/>
    protected override bool ConvertBack(Style? value, object? parameter, CultureInfo culture)
        => StyleWhenTrue == value;
}