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
using BadEcho.Game.Effects;
using BadEcho.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides a multi-channel signed distance field font.
/// </summary>
public sealed class DistanceFieldFont
{
    private readonly GraphicsDevice _device;
    private readonly Dictionary<char, FontGlyph> _glyphs;
    private readonly Dictionary<CharacterPair, KerningPair> _kernings;
    private readonly List<StaticModel> _models = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceFieldFont"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the font's models.</param>
    /// <param name="atlas">The texture of the font's atlas, generated with distance fields.</param>
    /// <param name="characteristics">The font's characteristics and metrics.</param>
    /// <param name="glyphs">A mapping between unicode characters and their typographic representations.</param>
    /// <param name="kernings">A mapping between unicode character pairs and the adjustments of space between them.</param>
    public DistanceFieldFont(GraphicsDevice device,
                             Texture2D atlas,
                             FontCharacteristics characteristics,
                             Dictionary<char, FontGlyph> glyphs,
                             Dictionary<CharacterPair, KerningPair> kernings)
    {
        Require.NotNull(device, nameof(device));
        Require.NotNull(atlas, nameof(atlas));
        Require.NotNull(characteristics, nameof(characteristics));
        Require.NotNull(glyphs, nameof(glyphs));
        Require.NotNull(kernings, nameof(kernings));

        _device = device;
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
    /// Draws modeled text using this font to the screen.
    /// </summary>
    public void Draw()
    {
        var projection =
            Matrix.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, -1);
        
        var effect = new DistanceFieldFontEffect(_device)
                     {
                         WorldViewProjection = projection,
                         AtlasSize = new Vector2(Characteristics.Width, Characteristics.Height),
                         DistanceRange = Characteristics.DistanceRange,
                         Texture = Atlas
                     };

        foreach (var model in _models)
        {
            model.Draw(effect);
        }
    }

    /// <summary>
    /// Generates a model required to render the specified text when it is being submitted for drawing.
    /// </summary>
    /// <param name="text">The text to prepare a model for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    public void AddModel(string text, Vector2 position, Color color, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var fontData = new FontModelData(this, color);

        fontData.AddText(text, position, scale);

        _models.Add(new StaticModel(_device, Atlas, fontData));
    }

    /// <summary>
    /// Clears previously generated font models.
    /// </summary>
    public void ClearModels()
        => _models.Clear();
    
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
    /// Determines the next advance width to apply to the cursor given the specified character sequence and direction.
    /// </summary>  
    /// <param name="current">The character the cursor is currently positioned at.</param>
    /// <param name="next">
    /// The next character to advance the cursor to, or null if the current character was the last one.
    /// </param>
    /// <param name="advanceDirection">The direction to advance the cursor.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <returns>The next advance width to apply to the current cursor position.</returns>
    public Vector2 GetNextAdvance(char current, char? next, Vector2 advanceDirection, float scale)
    {
        FontGlyph currentGlyph = FindGlyph(current);
        Vector2 advance = advanceDirection * currentGlyph.Advance * scale;

        if (next.HasValue && _kernings.TryGetValue(new CharacterPair(current, next.Value), out KerningPair? kerning))
            advance += advanceDirection * kerning.Advance * scale;

        return advance;
    }
}