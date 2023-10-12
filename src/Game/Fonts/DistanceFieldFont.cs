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
    /// Generates a model required to render the specified text when it is being submitted for drawing.
    /// </summary>
    /// <param name="text">The text to prepare a model for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <returns>A <see cref="IModelRenderer"/> instance that will render the model.</returns>
    public IModelRenderer AddModel(string text, Vector2 position, Color color, float scale)
        => AddModel(text, position, color, scale, false);

    /// <summary>
    /// Generates a model required to render the specified text when it is being submitted for drawing.
    /// </summary>
    /// <param name="text">The text to prepare a model for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <param name="optimizeForSmallText">
    /// Value indicating if the text should be rendered using a shader optimized for smaller text, the use of which
    /// helps to avoid the artifacting normally observed when attempting to render signed distance fonts at small scale.
    /// </param>
    /// <returns>A <see cref="IModelRenderer"/> instance that will render the model.</returns>
    /// <remarks>
    /// If text being rendered with the default shader exhibit artifacts, it is recommended to pass <c>true</c> for
    /// <c>optimizeForSmallText</c>. The small-optimized shader is also able to render larger text, however the additional
    /// thickness applied in order to account for artifacting will also become more noticeable as the scale increases.
    /// </remarks>
    public IModelRenderer AddModel(string text, Vector2 position, Color color, float scale, bool optimizeForSmallText)
    {
        var fontData = new FontModelData(this, color);

        return AddModel(fontData, text, position, scale, optimizeForSmallText);
    }

    /// <summary>
    /// Generates a model required to render the specified text when it is being submitted for drawing.
    /// </summary>
    /// <param name="text">The text to prepare a model for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="fillColor">The fill color of the text.</param>
    /// <param name="strokeColor">The stroke color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <returns>A <see cref="IModelRenderer"/> instance that will render the model.</returns>
    public IModelRenderer AddModel(string text, Vector2 position, Color fillColor, Color strokeColor, float scale)
        => AddModel(text, position, fillColor, strokeColor, scale, false);

    /// <summary>
    /// Generates a model required to render the specified text when it is being submitted for drawing.
    /// </summary>
    /// <param name="text">The text to prepare a model for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="fillColor">The fill color of the text.</param>
    /// <param name="strokeColor">The stroke color of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <param name="optimizeForSmallText">
    /// Value indicating if the text should be rendered using a shader optimized for smaller text, the use of which
    /// helps to avoid the artifacting normally observed when attempting to render signed distance fonts at small scale.
    /// </param>
    /// <returns>A <see cref="IModelRenderer"/> instance that will render the model.</returns>
    /// <remarks>
    /// If text being rendered with the default shader exhibit artifacts, it is recommended to pass <c>true</c> for
    /// <c>optimizeForSmallText</c>. The small-optimized shader is also able to render larger text, however the additional
    /// thickness applied in order to account for artifacting will also become more noticeable as the scale increases.
    /// </remarks>
    public IModelRenderer AddModel(string text, 
                                   Vector2 position, 
                                   Color fillColor, 
                                   Color strokeColor, 
                                   float scale, 
                                   bool optimizeForSmallText)
    {
        var fontData = new FontModelData(this, fillColor, strokeColor);

        return AddModel(fontData, text, position, scale, optimizeForSmallText);
    }

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

    private IModelRenderer AddModel(FontModelData fontData, 
                                    string text, 
                                    Vector2 position, 
                                    float scale,
                                    bool optimizeForSmallText)
    {
        if (!string.IsNullOrEmpty(text))
            fontData.AddText(text, position, scale);

        return new DistanceFieldFontRenderer(this, fontData, optimizeForSmallText);
    }

    /// <summary>
    /// Provides a renderer of signed distance field font models using shaders appropriate to the size of the text.
    /// </summary>
    private sealed class DistanceFieldFontRenderer : IModelRenderer, IDisposable
    {
        private readonly DistanceFieldFont _font;
        private readonly StaticModel _model;
        private readonly FontModelData _fontData;
        private readonly bool _useSmallShader;
        private readonly bool _useStrokedShader;

        private SizeF? _size;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceFieldFontRenderer"/> class.
        /// </summary>
        /// <param name="font">The multi-channel signed distance font to render.</param>
        /// <param name="fontData">Vertex data required to render the model.</param>
        /// <param name="useSmallShader">Value indicating whether a shader optimized for rendering small text is used.</param>
        public DistanceFieldFontRenderer(DistanceFieldFont font, FontModelData fontData, bool useSmallShader)
        {
            _font = font;
            _fontData = fontData;
            _model = new StaticModel(font._device, font.Atlas, fontData);
            _useSmallShader = useSmallShader;
            _useStrokedShader = !fontData.FillOnly;
        }

        /// <inheritdoc/>
        public SizeF Size
        {
            get
            {
                _size ??= _fontData.MeasureSize();

                return _size.Value;
            }
        }

        /// <inheritdoc/>
        public void Draw()
            => Draw(Matrix.Identity);

        /// <inheritdoc/>
        public void Draw(Matrix view)
        {
            GraphicsDevice device = _font._device;

            var projection =
                Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0, -1);

            var effect = new DistanceFieldFontEffect(device)
                         {
                             WorldViewProjection = view * projection,
                             AtlasSize = new Vector2(_font.Characteristics.Width, _font.Characteristics.Height),
                             DistanceRange = _font.Characteristics.DistanceRange,
                             Texture = _font.Atlas
                         };

            effect.CurrentTechnique = GetTechnique(effect);

            _model.Draw(effect);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
                return;

            _model.Dispose();

            _disposed = true;
        }

        private EffectTechnique GetTechnique(Effect effect) => (_useSmallShader, _useStrokedShader)
            switch
            {
                (false, false) => effect.Techniques[DistanceFieldFontEffect.LargeTextTechnique],
                (true, false) => effect.Techniques[DistanceFieldFontEffect.SmallTextTechnique],
                (false, true) => effect.Techniques[DistanceFieldFontEffect.LargeStrokedTextTechnique],
                (true, true) => effect.Techniques[DistanceFieldFontEffect.SmallStrokedTextTechnique]
            };
    }
}