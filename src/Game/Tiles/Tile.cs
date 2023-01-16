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
        IdWithFlags = idWithFlags;
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    /// <summary>
    /// Gets the global identifier for this tile, along with its flip flags.
    /// </summary>
    /// <remarks>
    /// Refer to the <see cref="Id"/> property, which has the flip flag data stripped from the value, for a useable identifier.
    /// </remarks>
    public uint IdWithFlags
    { get; }

    /// <summary>
    /// Gets the identifier of this tile, unique across all tiles found in the tile sets used by a tile map.
    /// </summary>
    /// <remarks>
    /// The identifier for a tile is based on <see cref="IdWithFlags"/> with the most significant 4 bits (nibble) cleared.
    /// </remarks>
    public int Id
        => (int) (IdWithFlags & 0xFFFFFFF);

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
        => Flips.HasFlag(TileFlips.TwiceRotatedHexagon);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped anti-diagonally when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedAntiDiagonally"/>
    public bool IsFlippedAntiDiagonally
        => Flips.HasFlag(TileFlips.FlippedAntiDiagonally);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped in the vertical direction when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedVertically"/>
    public bool IsFlippedVertically
        => Flips.HasFlag(TileFlips.FlippedVertically);

    /// <summary>
    /// Gets a value indicating whether this tile should be flipped in the horizontal direction when rendered.
    /// </summary>
    /// <seealso cref="TileFlips.FlippedHorizontally"/>.
    public bool IsFlippedHorizontally
        => Flips.HasFlag(TileFlips.FlippedHorizontally);

    /// <summary>
    /// Gets the directions in which this tile is to be flipped and/or rotated when being rendered.
    /// </summary>
    private TileFlips Flips
        => (TileFlips) IdWithFlags;
}
