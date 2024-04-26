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

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to writing tile-related assets to the content pipeline.
/// </summary>
internal static class ContentWriterExtensions
{
    /// <summary>
    /// Writes the provided extensible asset's custom properties to the content pipeline.
    /// </summary>
    /// <param name="writer">The current content writer.</param>
    /// <param name="asset">An asset containing custom properties to write.</param>
    public static void WriteProperties(this ContentWriter writer, ExtensibleAsset asset)
    {
        Require.NotNull(writer, nameof(writer));
        Require.NotNull(asset, nameof(asset));

        WriteProperties(writer, asset.CustomStringProperties, (w, v) => w.Write(v));
        WriteProperties(writer, asset.CustomBoolProperties, (w, v) => w.Write(v));
        WriteProperties(writer, asset.CustomColorProperties, (w, v) => w.Write(v));
        WriteProperties(writer, asset.CustomIntProperties, (w, v) => w.Write(v));
        WriteProperties(writer, asset.CustomFloatProperties, (w, v) => w.Write(v));
        WriteProperties(writer, asset.CustomFileProperties, (w, v) => w.Write(v));
    }

    private static void WriteProperties<T>(ContentWriter writer,
                                           IReadOnlyDictionary<string, T> customProperties,
                                           Action<ContentWriter, T> valueWriter)
    {
        // Need to record the number of custom properties in order to properly direct the reader.
        writer.Write(customProperties.Count);

        foreach ((string name, T value) in customProperties)
        {
            writer.Write(name);
            valueWriter(writer, value);
        }
    }
}
