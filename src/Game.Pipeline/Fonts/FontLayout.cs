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

using System.Text.Json.Serialization;
using BadEcho.Game.Fonts;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides a class for holding deserialized multi-channel signed distance field font layout data.
/// </summary>
internal sealed class FontLayout
{
    /// <summary>
    /// Gets the characteristics of the font.
    /// </summary>
    [JsonPropertyName("atlas")]
    public FontCharacteristics Characteristics
    { get; init; } = new();

    /// <summary>
    /// Gets the typographic representations of the unicode characters in the font atlas's character set.
    /// </summary>
    public IEnumerable<FontGlyph> Glyphs
    { get; init; } = Enumerable.Empty<FontGlyph>();

    /// <summary>
    /// Gets the adjustments of space between each of the unicode characters in the font atlas's character set.
    /// </summary>
    public IEnumerable<FontLayoutKerning> Kerning
    { get; init; } = Enumerable.Empty<FontLayoutKerning>();
}
