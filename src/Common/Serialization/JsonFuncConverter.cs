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

using System.Text.Json;
using System.Text.Json.Serialization;
using BadEcho.Properties;

namespace BadEcho.Serialization;

/// <summary>
/// Provides an encapsulated conversion of objects to or from numbers in JSON.
/// </summary>
/// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
public sealed class JsonIntFuncConverter<T> : JsonConverter<T>
{
    private readonly Func<int, T> _readConvert;
    private readonly Func<T, int> _writeConvert;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonIntFuncConverter{T}"/> class.
    /// </summary>
    /// <param name="readConvert">The method used to convert numeric values when reading from JSON.</param>
    /// <param name="writeConvert">The method used to convert to numeric values when writing to JSON.</param>
    public JsonIntFuncConverter(Func<int, T> readConvert, Func<T, int> writeConvert)
    {
        Require.NotNull(readConvert, nameof(readConvert));
        Require.NotNull(writeConvert, nameof(writeConvert));

        _readConvert = readConvert;
        _writeConvert = writeConvert;
    }

    /// <inheritdoc/>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
            throw new JsonException(Strings.JsonNotNumber);

        int number = reader.GetInt32();

        return _readConvert(number);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Require.NotNull(writer, nameof(writer));

        writer.WriteNumberValue(_writeConvert(value));
    }
}
