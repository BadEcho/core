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

using BadEcho.Extensions;
using BadEcho.Properties;
using Microsoft.Xna.Framework;
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

    /// <summary>
    /// Retrieves the typographic representation of the specified unicode character from this font.
    /// </summary>
    /// <param name="character">The unique character to retrieve the typographic representation for.</param>
    /// <returns>The <see cref="FontGlyph"/> representing <c>character</c>.</returns>
    public FontGlyph FindGlyph(char character)
    {
        if (!_glyphs.TryGetValue(character, out FontGlyph? glyph))
            throw new ArgumentException(Strings.GlyphNotInFont.InvariantFormat(character), nameof(character));

        return glyph;
    }

    /// <summary>
    /// Creates 3D modeling data for text using this font that can be rendered at a later time.
    /// </summary>
    /// <param name="text">The text to prepare modeling data for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <returns>
    /// A <see cref="FontModelData"/> instance containing modeling data that, when rendered, will draw <c>text</c> to the screen.
    /// </returns>
    public FontModelData CreateModelData(string text, Vector2 position, Color color, float scale)
    {
        var modelData = new FontModelData(color);

        if (string.IsNullOrEmpty(text))
            return modelData;

        var advanceDirection = new Vector2(1, 0);
        // The vertical direction is the vector that's perpendicular to our advance direction.
        var verticalDirection = -1 * new Vector2(advanceDirection.Y, -advanceDirection.X);

        // The provided position vector value specifies where the top-left corner of the text should be placed.
        // The generated signed distance glyph plane coordinates, however, are meant to be applied relative to the baseline cursor.
        // Therefore, we subtract the distance between the baseline and ascender line from our initial cursor, giving us a cursor now
        // positioned at the baseline.
        Vector2 cursorStart = position + verticalDirection * scale * Characteristics.Ascender * -1;
        Vector2 cursor = cursorStart;
        Vector2 scaledAdvance = advanceDirection * scale;

        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];
            FontGlyph glyph = FindGlyph(character);

            if (!char.IsWhiteSpace(character)) 
                modelData.AddGlyph(glyph, Characteristics, cursor, scaledAdvance);

            char? nextCharacter = i < text.Length - 1 ? text[i + 1] : null;

            cursor += GetNextAdvance(character, nextCharacter, advanceDirection, scale);
        }

        return modelData;
    }
    
    private Vector2 GetNextAdvance(char current, char? next, Vector2 advanceDirection, float scale)
    {
        FontGlyph currentGlyph = FindGlyph(current);
        Vector2 advance = advanceDirection * currentGlyph.Advance * scale;

        if (next.HasValue && _kernings.TryGetValue(new CharacterPair(current, next.Value), out KerningPair? kerning))
            advance += advanceDirection * kerning.Advance * scale;

        return advance;
    }
}