//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BadEcho.Properties;

namespace BadEcho.Serialization;

/// <summary>
/// Provides a class for converting a range of numeric values to or from JSON.
/// </summary>
/// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
/// <remarks>
/// <para>
/// This converter will produce a range of numeric values as described by the JSON input; refer to the example
/// for how these ranges should be formatted in JSON.
/// </para>
/// </remarks>
/// <example>Given the following JSON input:
/// <code>
/// "ranges": [
///     {
///         "start": 5,
///         "end": 20
///     },
///     {
///         "start": 80,
///         "end": 200
///     }
/// ]
/// </code>
/// <para>
/// The output from this converter will be a sequence containing all numbers from 5 to 20, as well as all numbers 
/// from 80 to 200. 
/// </para>
/// <para>
/// The names used for the <c>start</c> and <c>end</c> properties are inconsequential; all that matters is the order.
/// </para>
/// </example>
public sealed class JsonRangeConverter<T> : JsonConverter<IEnumerable<T>>
    where T : unmanaged, IConvertible
{
    /// <inheritdoc/>
    public override IEnumerable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException(Strings.JsonNotStartArray);

        var ranges = new List<T>();

        reader.Read();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            ranges.AddRange(ReadRange(ref reader));
            reader.Read();
        }

        return ranges;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
    {
        Require.NotNull(writer, nameof(writer));
        Require.NotNull(value, nameof(value));
        
        writer.WriteStartArray();

        var range = new List<int>();

        foreach (T valueInRange in value)
        {
            int numberInRange = valueInRange.ToInt32(CultureInfo.InvariantCulture);

            // If this number comes immediately after the previous one, we add it to the current range.
            if (range.Count == 0 || numberInRange - range[^1] <= 1)
                range.Add(numberInRange);
            else
            {
                WriteRange(writer, range[0], range[^1]);
                range = [numberInRange];
            }
        }

        if (range.Count > 0)
            WriteRange(writer, range[0], range[^1]);

        writer.WriteEndArray();
    }

    private static List<T> ReadRange(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException(Strings.JsonNotStartObject);

        var range = new List<T>();

        int start = ReadRangeExtremum(ref reader);
        int end = ReadRangeExtremum(ref reader);

        reader.Read();

        for (int i = start; i <= end; i++)
        {   
            T value = (T) Convert.ChangeType(i, typeof(T), CultureInfo.InvariantCulture);
            range.Add(value);
        }

        return range;
    }

    private static int ReadRangeExtremum(ref Utf8JsonReader reader)
    {
        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException(Strings.JsonMalformedText);

        reader.Read();

        if (reader.TokenType != JsonTokenType.Number)
            throw new JsonException(Strings.JsonExtremumValueNotNumber);

        return reader.GetInt32();
    }

    private static void WriteRange(Utf8JsonWriter writer, int start, int end)
    {
        writer.WriteStartObject();

        writer.WriteNumber(nameof(start), start);
        writer.WriteNumber(nameof(end), end);

        writer.WriteEndObject();
    }
}
