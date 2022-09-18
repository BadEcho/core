//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
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
    /// <param name="reader">The current game content reader.</param>
    /// <returns>A mapping of the content's custom property names to their values, read from <c>reader</c>.</returns>
    public static IReadOnlyDictionary<string, string> ReadProperties(this ContentReader reader)
    {
        Require.NotNull(reader, nameof(reader));

        var customProperties = new Dictionary<string, string>();
        var customPropertiesToRead = reader.ReadInt32();

        while (customPropertiesToRead > 0)
        {
            var propertyName = reader.ReadString();
            var propertyValue = reader.ReadString();

            customProperties.Add(propertyName, propertyValue);
            customPropertiesToRead--;
        }

        return customProperties;
    }
}
