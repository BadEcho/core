// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

using BadEcho.Properties;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BadEcho.Serialization;

/// <summary>
/// Provides a class for converting a flattened set of objects to or from JSON.
/// </summary>
/// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
public sealed class JsonFlattenedObjectConverter<T> : JsonConverter<T>
{
    private readonly ConditionalWeakTable<JsonSerializerOptions, JsonSerializerOptions> _passThruOptions = [];
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

        JsonObject baseObject = JsonNode.Parse(ref reader)?.AsObject()
                                ?? throw new JsonException(Strings.JsonNodeIsNull);

        for (int i = 1; i < _elementsToSquash; i++)
        {
            reader.Read();

            JsonObject nextObject = JsonNode.Parse(ref reader)?.AsObject()
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
    {
        Require.NotNull(writer, nameof(writer));

        JsonSerializer.Serialize(writer, value, GetPassThruOptions(options));
    }

    /// <summary>
    /// Gets a copy of the provided options that excludes this converter, allowing <typeparamref name="T"/> to be
    /// handled by its default conversion without recursing back into this converter.
    /// </summary>
    /// <remarks>
    /// Copies are cached per options instance, as options in use by the serializer are immutable and are
    /// typically reused across many conversions.
    /// </remarks>
    private JsonSerializerOptions GetPassThruOptions(JsonSerializerOptions options)
    {   // The ConditionalWeakTable holds the options weakly, so short-lived options instances won't be kept alive.
        return _passThruOptions.GetValue(options, CreatePassThruOptions);
    }

    private JsonSerializerOptions CreatePassThruOptions(JsonSerializerOptions options)
    {
        var passThruOptions = new JsonSerializerOptions(options);
        // The default converter for type T will most likely be this converter, so we'll want to remove ourselves from the provided options.
        passThruOptions.Converters.Remove(this);

        return passThruOptions;
    }
}
