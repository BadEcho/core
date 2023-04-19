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

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides configuration data for a texture atlas.
/// </summary>
public sealed class TextureAtlasAsset
{
    /// <summary>
    /// Gets or sets the path to the file containing the atlas's texture.
    /// </summary>
    public string TexturePath
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets the collection of regions the atlas comprises.
    /// </summary>
    public IReadOnlyCollection<TextureRegionAsset> Regions 
    { get; init; } = new List<TextureRegionAsset>();
}
