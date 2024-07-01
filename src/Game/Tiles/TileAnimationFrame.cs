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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Represents a frame in an animation sequence of a tile.
/// </summary>
/// <param name="TileId">
/// The local identifier of the tile within a tile set to display when this frame is active.
/// </param>
/// <param name="Duration">The amount of time this frame should be displayed.</param>
public sealed record TileAnimationFrame(int TileId, TimeSpan Duration);