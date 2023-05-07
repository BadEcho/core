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

    /// <summary>
    /// Gets or sets the font used for this label's text.
    /// </summary>
    public SpriteFont? Font
    {
        get => _font;
        set => RemeasureIfChanged(ref _font, value);
    }

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

        return Font.MeasureString(Text).ToPoint();
    }

    /// <inheritdoc />
    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (Font == null)
        {
            Logger.Debug(Strings.LabelNoFont);
            return;
        }

        spriteBatch.DrawString(Font, Text, ContentBounds.Location.ToVector2(), Color.White);
    }
}