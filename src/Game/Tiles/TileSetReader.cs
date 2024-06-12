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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a reader of raw tile set content from the content pipeline.
/// </summary>
public sealed class TileSetReader : ContentTypeReader<TileSet>
{
    /// <summary>
    /// Reads a tile set instance from the content pipeline.
    /// </summary>
    /// <param name="input">The content pipeline reader.</param>
    /// <returns>A <see cref="TileSet"/> instance read from <c>input</c>.</returns>
    public static TileSet Read(ContentReader input)
    {
        Require.NotNull(input, nameof(input));

        Texture2D? texture = null;

        var hasImage = input.ReadBoolean();

        if (hasImage)
            texture = input.ReadExternalReference<Texture2D>();

        var tileWidth = input.ReadInt32();
        var tileHeight = input.ReadInt32();
        var tileCount = input.ReadInt32();
        var columns = input.ReadInt32();
        var spacing = input.ReadInt32();
        var margin = input.ReadInt32();
        var customProperties = input.ReadProperties();

        var tileSet = new TileSet(texture,
                                  new Size(tileWidth, tileHeight),
                                  tileCount,
                                  columns,
                                  customProperties)
                      {
                          Spacing = spacing,
                          Margin = margin
                      };

        ReadTiles(input, tileSet);

        return tileSet;
    }

    /// <inheritdoc />
    protected override TileSet Read(ContentReader input, TileSet existingInstance)
        => Read(input);

    private static void ReadTiles(ContentReader input, TileSet tileSet)
    {
        var tilesToRead = input.ReadInt32();

        while (tilesToRead > 0)
        {
            tilesToRead--;

            var id = input.ReadInt32();
            var hasImage = input.ReadBoolean();

            if (!hasImage)
                continue;

            var texture = input.ReadExternalReference<Texture2D>();

            tileSet.AddTile(id, texture);
        }
    }
}
