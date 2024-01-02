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

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a control that displays an image.
/// </summary>
public sealed class Image : Control
{
    private IVisualRegion? _visual;

    /// <summary>
    /// Gets or sets the visual component data composing the image.
    /// </summary>
    public IVisualRegion? Visual
    {
        get => _visual;
        set => RemeasureIfChanged(ref _visual, value);
    }

    /// <inheritdoc />
    protected override Size MeasureCore(Size availableSize)
    {
        if (Visual == null)
            return Size.Empty;

        int width = Visual.Size.Width;
        int height = Visual.Size.Height;

        float aspectRatio = (float) Visual.Size.Width / Visual.Size.Height;

        if (Width != null && Height == null) 
            height = (int) aspectRatio * Width.Value;

        else if (Height != null && Width == null)
            width = (int) aspectRatio * Height.Value;

        return new Size(width, height);
    }

    /// <inheritdoc />
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        Visual?.Draw(spriteBatch, ContentBounds);
    }
}
