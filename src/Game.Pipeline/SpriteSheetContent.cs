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

using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides the raw data for sprite sheet content.
/// </summary>
public sealed class SpriteSheetContent : Texture2DContent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetContent"/> class.
    /// </summary>
    /// <param name="content">The texture containing the individual frames that compose the sprite sheet.</param>
    /// <param name="asset">The configuration data for the sprite sheet.</param>
    public SpriteSheetContent(Texture2DContent content, SpriteSheetAsset asset)
    { 
        Require.NotNull(asset, nameof(asset));
        Require.NotNull(content, nameof(content));
        // This copies over the primary texture data.
        Faces[0] = content.Faces[0];

        Asset = asset;
    }

    /// <summary>
    /// Gets the configuration data for the sprite sheet.
    /// </summary>
    public SpriteSheetAsset Asset
    { get; }
}
