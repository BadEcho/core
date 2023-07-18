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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a selectable item inside a <see cref="Menu"/> control.
/// </summary>
public sealed class MenuItem : Control
{
    private readonly Label _innerLabel;
    private readonly Image _innerImage;
    private readonly Grid _innerPanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    public MenuItem()
    {
        _innerImage = new Image
                      {
                          HorizontalAlignment = HorizontalAlignment.Center,
                          VerticalAlignment = VerticalAlignment.Center,
                          Column = 0,
                          Parent = this
                      };

        _innerLabel = new Label
                      {
                          HorizontalAlignment = HorizontalAlignment.Center,
                          VerticalAlignment = VerticalAlignment.Center,
                          Column = 1,
                          Parent = this
                      };

        _innerPanel = new Grid
                      {
                          Parent = this
                      };

        _innerPanel.AddChild(_innerImage);
        _innerPanel.AddChild(_innerLabel);
    }

    /// <summary>
    /// Occurs when the user has selected this menu item.
    /// </summary>
    public event EventHandler? Selected;

    /// <summary>
    /// Gets or sets the text of the menu item.
    /// </summary>
    public string Text
    {
        get => _innerLabel.Text;
        set => _innerLabel.Text = value;
    }

    /// <summary>
    /// Gets or sets the font used for this menu item's text.
    /// </summary>
    public SpriteFont? Font
    {
        get => _innerLabel.Font;
        set => _innerLabel.Font = value;
    }

    /// <summary>
    /// Gets or sets the color of the font used for this menu item's text.
    /// </summary>
    public Color FontColor
    {
        get => _innerLabel.FontColor;
        set => _innerLabel.FontColor = value;
    }

    /// <summary>
    /// Gets or sets the visual component data for an optional image to display alongside the menu item's text.
    /// </summary>
    public IVisualRegion? Image
    {
        get => _innerImage.Visual;
        set => _innerImage.Visual = value;
    }

    /// <summary>
    /// Gets or sets the specific width of the image displayed alongside the menu item's text.
    /// </summary>
    public int? ImageWidth
    {
        get => _innerImage.Width;
        set => _innerImage.Width = value;
    }

    /// <summary>
    /// Gets or sets the specific height of the image displayed alongside the menu item's text.
    /// </summary>
    public int? ImageHeight
    {
        get => _innerImage.Height;
        set => _innerImage.Height = value;
    }

    /// <summary>
    /// Selects the menu item.
    /// </summary>
    public void Select() 
        => Selected?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    protected override Size MeasureCore(Size availableSize)
    {
        _innerLabel.Margin = Image != null ? new Thickness(10, 0, 0, 0) : new Thickness(0);

        _innerPanel.Measure(availableSize);

        return _innerPanel.DesiredSize;
    }

    /// <inheritdoc/>
    protected override void ArrangeCore()
    {
        base.ArrangeCore();

        _innerPanel.Arrange(ContentBounds);
    }

    /// <inheritdoc />
    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        _innerPanel.Draw(spriteBatch);
    }
}
