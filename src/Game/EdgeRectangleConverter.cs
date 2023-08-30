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
using System.Text.Json.Serialization;
using BadEcho.Properties;

namespace BadEcho.Game;

/// <summary>
/// Provides a class for converting an edge-specified rectangle to or from JSON.
/// </summary>
public sealed class EdgeRectangleConverter : JsonConverter<RectangleF> 
{
    /// <inheritdoc/>
    public override RectangleF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException(Strings.JsonNotStartObject);

        float left = ReadEdge(ref reader);
        float top = ReadEdge(ref reader);
        float right = ReadEdge(ref reader);
        float bottom = ReadEdge(ref reader);

        reader.Read();

        return new RectangleF(left, top, right - left, bottom - top);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RectangleF value, JsonSerializerOptions options)
    {
        Require.NotNull(writer, nameof(writer));

        writer.WriteStartObject();
        WriteEdges(writer, value.Left, value.Top, value.Right, value.Bottom);
        writer.WriteEndObject();
    }

    private static float ReadEdge(ref Utf8JsonReader reader)
    {
        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException(Strings.JsonMalformedText);

        reader.Read();

        return reader.GetSingle();
    }

    private static void WriteEdges(Utf8JsonWriter writer, float left, float top, float right, float bottom)
    {
        writer.WriteNumber(nameof(left), left);
        writer.WriteNumber(nameof(top), top);
        writer.WriteNumber(nameof(right), right);
        writer.WriteNumber(nameof(bottom), bottom);
    }
}
