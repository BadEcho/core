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

using System.Text.Json.Serialization;
using BadEcho.Serialization;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides configuration data for a multi-channel signed distance field font asset.
/// </summary>
public sealed class DistanceFieldFontAsset
{
    /// <summary>
    /// Gets or sets the path to the font file (.ttf/.otf) to create an atlas for.
    /// </summary>
    public string FontPath
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets the character set that will be included in the font atlas.
    /// </summary>
    [JsonConverter(typeof(JsonRangeConverter<char>))]
    public IEnumerable<char>? CharacterSet
    { get; init; }
    
    /// <summary>
    /// Gets the size of the glyphs in the atlas, in pixels-per-em.
    /// </summary>
    public int Resolution
    { get; init; }

    /// <summary>
    /// Gets the distance field range in output pixels, which affects how far the distance field extends beyond the glyphs.
    /// </summary>
    public int Range
    { get; init; }
}
