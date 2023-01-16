//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a value converter that converts provided hexadecimal <see cref="string"/> values to
/// <see cref="Color"/> values.
/// </summary>
[ValueConversion(typeof(string), typeof(Color))]
public sealed class HexadecimalToColorConverter : ValueConverter<string,Color>
{
    /// <summary>
    /// Gets or sets the color returned for string values not properly expressing a valid ARGB color.
    /// </summary>
    public Color FallbackColor
    { get; set; } = Colors.Transparent;

    /// <inheritdoc/>
    protected override Color Convert(string value, object parameter, CultureInfo culture)
    {
        try
        {
            return (Color) ColorConverter.ConvertFromString(value);
        }
        catch (FormatException)
        {   // If this kind of exception is thrown, then input is not a valid color identifier.
            // Don't want to propagate errors in an IValueConverter, so we return the fallback color.
            return FallbackColor;
        }
    }

    /// <inheritdoc/>
    protected override string ConvertBack(Color value, object parameter, CultureInfo culture) 
        => $"{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
}