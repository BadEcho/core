// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Serialization;

/// <summary>
/// Provides a base class for converting a hierarchy of objects to or from JSON.
/// </summary>
/// <typeparam name="TTypeDescriptor">
/// A type of <see cref="Enum"/> whose integer value describes the type of object in JSON.
/// </typeparam>
/// <typeparam name="TBase">The base type of object handled by the converter.</typeparam>
public abstract class JsonPolymorphicConverter<TTypeDescriptor,TBase> : JsonConverter<TBase>
    where TTypeDescriptor : Enum
    where TBase : class
{
    private const string DEFAULT_TYPE_PROPERTY_NAME = "Type";

    /// <summary>
    /// Gets the expected property name of the number in JSON whose <typeparamref name="TTypeDescriptor"/> 
    /// representation is used to determine the specific type of <typeparamref name="TBase"/> instantiated.
    /// </summary>
    protected virtual string TypePropertyName 
        => DEFAULT_TYPE_PROPERTY_NAME;

    /// <summary>
    /// Gets the expected property name of the object in JSON containing the object data payload.
    /// </summary>
    protected abstract string DataPropertyName { get; }

    /// <inheritdoc/>
    public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException(Strings.JsonNotStartObject);

        Utf8JsonReader dataPropertyReader = default;
        int? typeValue = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException(Strings.JsonMalformedText);

            string propertyName = reader.GetString() ?? string.Empty;

            if (propertyName == TypePropertyName)
            {
                reader.Read();

                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException(Strings.JsonTypeValueNotNumber);

                typeValue = reader.GetInt32();
            }
            else
            {   // The payload is read from a stored copy once the type is known.
                if (propertyName == DataPropertyName)
                    dataPropertyReader = reader;

                // Moves past the property's value, whatever its kind or nesting.
                if (!reader.TrySkip())
                    throw new JsonException(Strings.JsonMalformedText);
            }
        }

        if (typeValue == null)
            throw new JsonException(Strings.JsonMissingProperty.InvariantFormat(TypePropertyName));

        if (dataPropertyReader.TokenType == JsonTokenType.None)
            throw new JsonException(Strings.JsonMissingProperty.InvariantFormat(DataPropertyName));

        return ReadDataProperty(ref dataPropertyReader, typeValue.Value.ToEnum<TTypeDescriptor>());
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
    {
        Require.NotNull(writer, nameof(writer));
        Require.NotNull(value, nameof(value));

        TTypeDescriptor typeDescriptor = DescriptorFromValue(value);

        writer.WriteStartObject();
            
        writer.WriteNumber(TypePropertyName, typeDescriptor.ToInt32());

        writer.WritePropertyName(DataPropertyName);
        JsonSerializer.Serialize(writer, value, value.GetType(), options);

        writer.WriteEndObject();
    }

    /// <summary>
    /// Reads and converts the JSON to a <typeparamref name="TBase"/>-derived type described by the provided
    /// <typeparamref name="TTypeDescriptor"/> enumeration.
    /// </summary>
    /// <param name="reader">The reader, positioned at the payload data of the object to read.</param>
    /// <param name="typeDescriptor">
    /// An enumeration value that specifies the type of <typeparamref name="TBase"/> to read.
    /// </param>
    /// <returns>The converted value.</returns>
    protected abstract TBase? ReadFromDescriptor(ref Utf8JsonReader reader, TTypeDescriptor typeDescriptor);

    /// <summary>
    /// Produces a <typeparamref name="TTypeDescriptor"/> value specifying a converted value's type.
    /// </summary>
    /// <param name="value">The converted value to create a type descriptor for.</param>
    /// <returns>
    /// A <typeparamref name="TTypeDescriptor"/> value that specifies the type of <c>value</c> in JSON.
    /// </returns>
    protected abstract TTypeDescriptor DescriptorFromValue(TBase value);

    private TBase? ReadDataProperty(ref Utf8JsonReader reader, TTypeDescriptor typeDescriptor)
    {
        reader.Read();

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException(Strings.JsonDataValueNotObject);

        return ReadFromDescriptor(ref reader, typeDescriptor);
    }
}