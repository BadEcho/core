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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a source of rectangular images, or tiles, for use in a tile map.
/// </summary>
public sealed class TileSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileSet"/> class.
    /// </summary>
    public TileSet(Texture2D texture, Point tileSize, int tileCount, int columns)
    {
        Texture = texture;
        TileSize = tileSize;
        TileCount = tileCount;
        Columns = columns;
    }

    /// <summary>
    ///  Gets the texture containing the individual tiles that compose the tile set.
    /// </summary>
    public Texture2D Texture
    { get; } 

    /// <summary>
    /// Gets size of an individual tile in the tile set.
    /// </summary>
    public Point TileSize
    { get; }

    /// <summary>
    /// Gets the number of tiles in the tile set.
    /// </summary>
    public int TileCount
    { get; }

    /// <summary>
    /// Gets the number of columns of tiles in the tile set.
    /// </summary>
    public int Columns
    { get; }

    /// <summary>
    /// Gets the space between the perimeter of the tiles composing the tile set and the edge of the texture.
    /// </summary>
    public int Margin
    { get; init; }

    /// <summary>
    /// Gets the space the individual tiles that compose the tile set.
    /// </summary>
    public int Spacing
    { get; init; }
}
