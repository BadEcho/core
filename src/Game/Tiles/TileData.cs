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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides configuration for a tile belonging to a tile set.
/// </summary>
public sealed class TileData : Extensible
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileData"/> class.
    /// </summary>
    /// <param name="id">The local identifier of this tile within its tile set.</param>
    /// <param name="texture">The texture for this tile, if one exists.</param>
    /// <param name="customProperties">The tile's custom properties.</param>
    public TileData(int id, Texture2D? texture, CustomProperties customProperties)
        : base(customProperties)
    {
        Id = id;
        Texture = texture;
    }

    /// <summary>
    /// Gets the local identifier of this tile within its tile set.
    /// </summary>
    public int Id
    { get; }

    /// <summary>
    /// Gets the texture for this tile, if one exists.
    /// </summary>
    /// <remarks>
    /// This will only be present here if the tile set this tile belongs to is a collection of images, as opposed to being
    /// based on a single image.
    /// </remarks>
    public Texture2D? Texture
    { get; }
}
