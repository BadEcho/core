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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a reference to a rectangular graphic image that exists within a grid of tile data.
/// </summary>
/// <remarks>
/// <para>
/// Tiles are the rectangular images that compose a tile map. The <see cref="Tile"/> class, however, contains no texture
/// data. This is because an individual tile is sourced from a tile set, which itself only needs to be loaded into memory once.
/// </para>
/// <para>
/// What this type does contain, is the necessary information that both identifies which tile in a tile set is being represented,
/// as well as where it is positioned in the grid of tile data it resides in.
/// </para>
/// </remarks>  
public sealed class Tile
{
    private readonly TileFlips _flips;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class.
    /// </summary>
    /// <param name="idWithFlags">The tile's global identifier along with its flip flags.</param>
    /// <param name="columnIndex">The index of the tile's column within the grid of tile data that contains it.</param>
    /// <param name="rowIndex">The index of the tile's row within the grid of tile data that contains it.</param>
    /// <remarks>
    /// The TMX map format encodes tile identifiers as unsigned integers with the highest four bits of the value representing
    /// the tile's "flip flags". This type stores these flags for future reference, and then clears the highest four bits, storing
    /// the resulting value as the actual identifier for the tile.
    /// </remarks>
    public Tile(uint idWithFlags, int columnIndex, int rowIndex)
    {
        _flips = (TileFlips) idWithFlags;
        // Clear the most significant 4 bits (nibble).
        Id = (int) (idWithFlags & 0xFFFFFFF);
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    /// <summary>
    /// Gets the identifier of this tile, unique across all tiles found in the tile sets used by a tile map.
    /// </summary>
    public int Id
    { get; }

    /// <summary>
    /// Gets the index of this tile's column within the grid of tile data that contains it.
    /// </summary>
    public int ColumnIndex
    { get; }

    /// <summary>
    /// Gets the index of this tile's row within the grid of tile data that contains it.
    /// </summary>
    public int RowIndex
    { get; }

    /// <summary>
    /// Gets a value indicating whether this tile, hopefully a hexagon, should be rotated 120 degrees clockwise when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.TwiceRotatedHexagon"/>
    public bool IsTwiceRotatedHexagon
        => _flips.HasFlag(TileFlips.TwiceRotatedHexagon);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped anti-diagonally when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedAntiDiagonally"/>
    public bool IsFlippedAntiDiagonally
        => _flips.HasFlag(TileFlips.FlippedAntiDiagonally);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped in the vertical direction when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedVertically"/>
    public bool IsFlippedVertically
        => _flips.HasFlag(TileFlips.FlippedVertically);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped in the horizontal direction when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedHorizontally"/>.
    public bool IsFlippedHorizontally
        => _flips.HasFlag(TileFlips.FlippedHorizontally);
}
