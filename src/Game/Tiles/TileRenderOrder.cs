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
/// Specifies the order in which tiles are rendered.
/// </summary>
public enum TileRenderOrder
{
    /// <summary>
    /// Tiles are drawn from left to right, from the uppermost to the lowermost tiles.
    /// </summary>
    RightDown,
    /// <summary>
    /// Tiles are drawn from left to right, from the lowermost to the uppermost tiles.
    /// </summary>
    RightUp,
    /// <summary>
    /// Tiles are drawn from right to left, from the uppermost to the lowermost tiles.
    /// </summary>
    LeftDown,
    /// <summary>
    /// Tiles are drawn from right to left, from the lowermost to the uppermost tiles.
    /// </summary>
    LeftUp
}
