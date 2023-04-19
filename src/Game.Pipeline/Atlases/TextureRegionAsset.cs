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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides configuration data for a region in a texture atlas.
/// </summary>
public sealed class TextureRegionAsset
{
    /// <summary>
    /// Gets the identifying name for the region, usually based on the file name it was sourced from.
    /// </summary>
    public string Name
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the bounding rectangle of the texture's region.
    /// </summary>
    public Rectangle SourceArea
    { get; init; }

    /// <summary>
    /// Gets the bounding rectangle of the center slice if this region uses 9-slice scaling; otherwise, an empty
    /// <see cref="Rectangle"/>.
    /// </summary>
    public Rectangle NineSliceArea
    { get; init; }
}
