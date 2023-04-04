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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Specifies the directions in which a tile is to be flipped and/or rotated from its original orientation when being rendered.
/// </summary>
[Flags]
public enum TileFlips : uint
{
    /// <summary>
    /// The tile should be rendered normally.
    /// </summary>
    None = 0,
    /// <summary>
    /// The tile, hopefully hexagonal, should be rotated 120 degrees clockwise, an amount that is twice that of a hexagon's
    /// angle of rotational symmetry.
    /// </summary>  
    TwiceRotatedHexagon = 0x10000000,
    /// <summary>
    /// The tile should be flipped anti-diagonally; that is, it is rotated 90 degrees clockwise, and then vertically flipped.
    /// </summary>
    /// <remarks>
    /// If the tile is hexagonal, it is simply rotated 60 degrees clockwise.
    /// </remarks>
    FlippedAntiDiagonally = 0x20000000,
    /// <summary>
    /// The tile should be flipped, or mirrored, in the vertical direction.
    /// </summary>
    FlippedVertically = 0x40000000,
    /// <summary>
    /// The tile should be flipped, or mirrored, in the horizontal direction.
    /// </summary>
    FlippedHorizontally = 0x80000000
}
