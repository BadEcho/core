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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides a multi-channel signed distance field font.
/// </summary>
public sealed class DistanceFieldFont
{
    private readonly Dictionary<char, FontGlyph> _glyphs;
    private readonly Dictionary<CharacterPair, KerningPair> _kernings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceFieldFont"/> class.
    /// </summary>
    /// <param name="atlas">The texture of the font's atlas, generated with distance fields.</param>
    /// <param name="characteristics">The font's characteristics and metrics.</param>
    /// <param name="glyphs">A mapping between unicode characters and their typographic representations.</param>
    /// <param name="kernings">A mapping between unicode character pairs and the adjustments of space between them.</param>
    public DistanceFieldFont(Texture2D atlas, 
                             FontCharacteristics characteristics, 
                             Dictionary<char, FontGlyph> glyphs,
                             Dictionary<CharacterPair, KerningPair> kernings)
    {
        Require.NotNull(atlas, nameof(atlas));
        Require.NotNull(characteristics, nameof(characteristics));
        Require.NotNull(glyphs, nameof(glyphs));
        Require.NotNull(kernings, nameof(kernings));

        Atlas = atlas;
        Characteristics = characteristics;
        _glyphs = glyphs;
        _kernings = kernings;
    }

    /// <summary>
    /// Gets the texture of this font's atlas, generated with distance fields.
    /// </summary>
    public Texture2D Atlas
    { get; }

    /// <summary>
    /// Gets this font's characteristics and metrics.
    /// </summary>
    public FontCharacteristics Characteristics
    { get; }
}
