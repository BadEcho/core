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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a source of rectangular images, or tiles, for use in a tile map.
/// </summary>
public sealed class TileSet : Extensible
{
    private readonly int _columns;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileSet"/> class.
    /// </summary>
    public TileSet(Texture2D texture, Size tileSize, int tileCount, int columns)
    {
        Texture = texture;
        TileSize = tileSize;
        TileCount = tileCount;
        _columns = columns;
    }

    /// <summary>
    ///  Gets the texture containing the individual tiles that compose this tile set.
    /// </summary>
    public Texture2D Texture
    { get; } 

    /// <summary>
    /// Gets size of an individual tile in this tile set.
    /// </summary>
    public Size TileSize
    { get; }

    /// <summary>
    /// Gets the number of tiles in this tile set.
    /// </summary>
    public int TileCount
    { get; }

    /// <summary>
    /// Gets the space between the perimeter of the tiles composing this tile set and the edge of the texture.
    /// </summary>
    public int Margin
    { get; init; }

    /// <summary>
    /// Gets the spacing between the individual tiles that compose this tile set.
    /// </summary>
    public int Spacing
    { get; init; }

    /// <summary>
    /// Gets the bounding rectangle for the identified tile within this tile set's texture.
    /// </summary>
    /// <param name="localId">The tile identifier localized to the tile set.</param>
    /// <returns>The bounding rectangle for the tile identified by <c>localId</c> within this tile set's texture.</returns>
    public Rectangle GetTileSourceArea(int localId)
    {
        int x = localId % _columns * (TileSize.Width + Spacing) + Margin;
        int y = localId / _columns * (TileSize.Height + Spacing) + Margin;

        return new Rectangle(x, y, TileSize.Width, TileSize.Height);
    }
}
