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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a text label user interface element.
/// </summary>
public sealed class Label : Control
{
    private string _text = string.Empty;
    private SpriteFont? _font;
    private DistanceFieldFont? _msdfFont;
    private IModelRenderer? _textRenderer;

    /// <summary>
    /// Gets or sets the font used for this label's text.
    /// </summary>
    public SpriteFont? Font
    {
        get => _font;
        set => RemeasureIfChanged(ref _font, value);
    }

    /// <summary>
    /// Gets or sets the font used for this label's text.
    /// </summary>
    public DistanceFieldFont? MsdfFont
    {
        get => _msdfFont;
        set => RemeasureIfChanged(ref _msdfFont, value);
    }

    /// <summary>
    /// Gets or sets the color of the font used for this label's text.
    /// </summary>
    public Color FontColor
    { get; set; }

    /// <summary>
    /// Gets or sets the size of the font used for this label's text.
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
        if (MsdfFont == null)
            return Size.Empty;

        // TODO: need to research how to convert font points to scale.
        _textRenderer = MsdfFont.AddModel(Text, Vector2.Zero, FontColor, 0.767f * FontSize);

        return (Size) _textRenderer.Size;
    }

    /// <inheritdoc />
    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (_textRenderer == null)
        {
            Logger.Debug(Strings.LabelNoFont);
            return;
        }

        Matrix textTranslation = Matrix.CreateTranslation(ContentBounds.X, ContentBounds.Y, 0);

        _textRenderer.Draw(textTranslation);
    }
}