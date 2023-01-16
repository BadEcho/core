//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
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

        var texture = input.ReadExternalReference<Texture2D>();
        var tileWidth = input.ReadInt32();
        var tileHeight = input.ReadInt32();
        var tileCount = input.ReadInt32();
        var columns = input.ReadInt32();
        var spacing = input.ReadInt32();
        var margin = input.ReadInt32();
        var customProperties = input.ReadProperties();

        return new TileSet(texture, new Size(tileWidth, tileHeight), tileCount, columns)
               {
                   Spacing = spacing,
                   Margin = margin,
                   CustomProperties = customProperties
               };
    }

    /// <inheritdoc />
    protected override TileSet Read(ContentReader input, TileSet existingInstance)
        => Read(input);
}
