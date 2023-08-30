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

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BadEcho.Properties;

namespace BadEcho.Serialization;

/// <summary>
/// Provides a class for converting a flattened set of objects to or from JSON.
/// </summary>
/// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
public sealed class JsonFlattenedObjectConverter<T> : JsonConverter<T>
{
    private static readonly JsonConverter<T> _FallbackConverter =
        (JsonConverter<T>) JsonSerializerOptions.Default.GetConverter(typeof(T));

    private readonly int _elementsToSquash;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFlattenedObjectConverter{T}"/> class.
    /// </summary>
    /// <param name="elementsToSquash">The number of JSON objects to flatten during read conversion.</param>
    public JsonFlattenedObjectConverter(int elementsToSquash)
    {
        _elementsToSquash = elementsToSquash;
    }

    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException(Strings.JsonNotStartObject);

        JsonObject? baseObject = JsonNode.Parse(ref reader)?.AsObject()
                                 ?? throw new JsonException(Strings.JsonNodeIsNull);

        for (int i = 1; i < _elementsToSquash; i++)
        {
            reader.Read();

            JsonObject? nextObject = JsonNode.Parse(ref reader)?.AsObject()
                                     ?? throw new JsonException(Strings.JsonNodeIsNull);

            foreach (var nextObjectProperty in nextObject.ToList())
            {
                nextObject.Remove(nextObjectProperty.Key);
                baseObject.Add(nextObjectProperty);
            }
        }

        var passThruOptions = new JsonSerializerOptions(options);

        passThruOptions.Converters.Remove(this);

        return baseObject.Deserialize<T>(passThruOptions);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => _FallbackConverter.Write(writer, value, options);
}
