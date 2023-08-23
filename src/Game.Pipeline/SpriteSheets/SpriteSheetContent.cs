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

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides the raw data for a sprite sheet asset.
/// </summary>
public sealed class SpriteSheetContent : ContentItem<SpriteSheetAsset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetContent"/> class.
    /// </summary>
    /// <param name="asset">The configuration data for the sprite sheet.</param>
    public SpriteSheetContent(SpriteSheetAsset asset)
        : base(asset)
    { }
}
