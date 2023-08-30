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

using BadEcho.Game.Fonts;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides the raw data for a multi-channel signed distance field font asset.
/// </summary>
public sealed class DistanceFieldFontContent : ContentItem<DistanceFieldFontAsset>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceFieldFontContent"/> class.
    /// </summary>
    public DistanceFieldFontContent(DistanceFieldFontAsset asset) 
        : base(asset)
    { }

    /// <summary>
    /// Gets or sets the path to the generated atlas image file.
    /// </summary>
    public string FontAtlasPath
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets font characteristics parsed from the generated layout data.
    /// </summary>
    public FontCharacteristics Characteristics
    { get; set; } = new();

    /// <summary>
    /// Gets a mapping between unicode characters and their typographic representations, parsed from
    /// the generated layout data.
    /// </summary>
    public Dictionary<char, FontGlyph> Glyphs
    { get; init; } = new();

    /// <summary>
    /// Gets a mapping between unicode character pairs and the adjustments of space between them, parsed from
    /// the generated layout data.
    /// </summary>
    public Dictionary<KerningPair, float> Kernings
    { get; init; } = new();
}
