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

using Microsoft.Xna.Framework.Content;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to reading tile-related content from the content pipeline.
/// </summary>
internal static class ContentReaderExtensions
{
    /// <summary>
    /// Reads an extensible content's custom properties from the content pipeline.
    /// </summary>
    /// <param name="reader">The current content reader.</param>
    /// <returns>The content's custom properties, read from <c>reader</c>.</returns>
    public static CustomProperties ReadProperties(this ContentReader reader)
    {
        Require.NotNull(reader, nameof(reader));

        var stringProperties = ReadProperties(reader, r => r.ReadString());
        var booleanProperties = ReadProperties(reader, r => r.ReadBoolean());
        var colorProperties = ReadProperties(reader, r => r.ReadColor());
        var integerProperties = ReadProperties(reader, r => r.ReadInt32());
        var floatProperties = ReadProperties(reader, r => r.ReadSingle());
        var fileProperties = ReadProperties(reader, r => new FileInfo(r.ReadString()));
        
        return new CustomProperties
               {
                   Booleans = booleanProperties,
                   Colors = colorProperties,
                   Files = fileProperties,
                   Floats = floatProperties,
                   Integers = integerProperties,
                   Strings = stringProperties
               };
    }

    private static Dictionary<string, T> ReadProperties<T>(ContentReader reader, Func<ContentReader, T> valueReader)
    {
        var customProperties = new Dictionary<string, T>();
        var customPropertiesToRead = reader.ReadInt32();

        while (customPropertiesToRead > 0)
        {
            var propertyName = reader.ReadString();
            var propertyValue = valueReader(reader);

            customProperties.Add(propertyName, propertyValue);
            customPropertiesToRead--;
        }

        return customProperties;
    }
}