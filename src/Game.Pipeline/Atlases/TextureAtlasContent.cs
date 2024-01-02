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

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides the raw data for a texture atlas asset.
/// </summary>
public sealed class TextureAtlasContent : ContentItem<TextureAtlasAsset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureAtlasContent"/> class.
    /// </summary>
    /// <param name="asset">The configuration data for a texture atlas asset.</param>
    public TextureAtlasContent(TextureAtlasAsset asset) 
        : base(asset)
    { }
}
