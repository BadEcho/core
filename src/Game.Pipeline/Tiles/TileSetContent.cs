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

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides the raw data for a tile set asset.
/// </summary>
public sealed class TileSetContent : ContentItem<TileSetAsset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileSetContent"/> class.
    /// </summary>
    /// <param name="asset">The configuration data for a tile set.</param>
    public TileSetContent(TileSetAsset asset) 
        : base(asset)
    { }
}
