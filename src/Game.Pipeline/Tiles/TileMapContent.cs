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

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides the raw data for a tile map asset.
/// </summary>
public sealed class TileMapContent : ContentItem<TileMapAsset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileMapContent"/> class.
    /// </summary>
    /// <param name="asset">The configuration data for a tile map.</param>
    public TileMapContent(TileMapAsset asset) 
        : base(asset)
    { }
}
