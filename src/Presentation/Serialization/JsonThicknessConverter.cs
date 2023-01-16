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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Serialization;

/// <summary>
/// Provides a converter of <see cref="Thickness"/> objects to and from JSON.
/// </summary>
public sealed class JsonThicknessConverter : JsonConverter<Thickness>
{
    /// <inheritdoc/>
    public override Thickness Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string thickness = reader.GetString() ?? string.Empty;

        double[] lengths = ParseLengths(thickness);
            
        return lengths.Length switch
        {
            1 => new Thickness(lengths[0]),
            2 => new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]),
            _ => new Thickness(lengths[0], lengths[1], lengths[2], lengths[3])
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Thickness value, JsonSerializerOptions options)
    {
        Require.NotNull(writer, nameof(writer));

        string separator = CultureInfo.InvariantCulture.TextInfo.ListSeparator;
        string thickness = $"{value.Left}{separator}{value.Top}{separator}{value.Right}{separator}{value.Bottom}";

        writer.WriteStringValue(thickness);
    }

    private static double[] ParseLengths(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new[] {0.0};

        string separator = CultureInfo.InvariantCulture.TextInfo.ListSeparator;
        string[] values = value.Split(separator);

        return values.Length switch
        {
            1 => new[] {double.Parse(values[0], CultureInfo.InvariantCulture)},
            2 => new[]
                 {
                     double.Parse(values[0], CultureInfo.InvariantCulture),
                     double.Parse(values[1], CultureInfo.InvariantCulture)
                 },
            4 => new[]
                 {
                     double.Parse(values[0], CultureInfo.InvariantCulture),
                     double.Parse(values[1], CultureInfo.InvariantCulture),
                     double.Parse(values[2], CultureInfo.InvariantCulture),
                     double.Parse(values[3], CultureInfo.InvariantCulture)
                 },
            _ => throw new JsonException(Strings.JsonThicknessInvalidThickness)
        };
    }
}