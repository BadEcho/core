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
using BadEcho.Game.Properties;
using BadEcho.Logging;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a text label user interface element.
/// </summary>
public sealed class Label : Control
{
    private string _text = string.Empty;
    private DistanceFieldFont? _font;
    private IModelRenderer? _textRenderer;

    /// <summary>
    /// Gets or sets the font used for this label's text.
    /// </summary>
    public DistanceFieldFont? Font
    {
        get => _font;
        set => RemeasureIfChanged(ref _font, value);
    }

    /// <summary>
    /// Gets or sets the color of the font used for this label's text.
    /// </summary>
    public Color FontColor
    { get; set; }

    /// <summary>
    /// Gets or sets the size of the font in points used for this label's text.
    /// </summary>
    public float FontSize
    { get; set; }

    /// <summary>
    /// Gets or sets the text contents of this label.
    /// </summary>
    public string Text
    {
        get => _text;
        set => RemeasureIfChanged(ref _text, value);
    }

    /// <inheritdoc />
    protected override Size MeasureCore(Size availableSize)
    {
        if (Font == null)
            return Size.Empty;
        
        _textRenderer = Font.AddModel(Text, Vector2.Zero, FontColor, FontSize / 0.767f);

        return (Size) _textRenderer.Size;
    }

    /// <inheritdoc />
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        if (_textRenderer == null)
        {
            Logger.Debug(Strings.LabelNoFont);
            return;
        }

        // UI elements are drawn immediately, as opposed to being deferred, so creating a new batch here won't be any significant effect.
        spriteBatch.End();

        Matrix textTranslation = Matrix.CreateTranslation(ContentBounds.X, ContentBounds.Y, 0);
        float alpha = 1.0f;

        if (spriteBatch.Effect != null)
        {
            textTranslation *= spriteBatch.Effect.MatrixTransform ?? Matrix.Identity;
            alpha = spriteBatch.Effect.Alpha;
        }

        _textRenderer.Draw(textTranslation, alpha);

        spriteBatch.Begin();
    }
}