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

using BadEcho.Game.Fonts;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a button user interface element, which can be clicked.
/// </summary>
public sealed class Button : Control
{
    private readonly Label _innerLabel;
    private readonly Image _innerImage;
    private readonly Grid _innerPanel;

    private bool _isPressed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    public Button()
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
    /// Occurs when the button has been clicked.
    /// </summary>
    public event EventHandler? Clicked;

    /// <summary>
    /// Gets or sets the text contents of this button.
    /// </summary>
    public string Text
    {
        get => _innerLabel.Text;
        set => _innerLabel.Text = value;
    }

    /// <summary>
    /// Gets or sets the font used for this button's text.
    /// </summary>
    public DistanceFieldFont? Font
    {
        get => _innerLabel.Font;
        set => _innerLabel.Font = value;
    }

    /// <summary>
    /// Gets or sets the color of the font used for this button's text.
    /// </summary>
    public Color FontColor
    {
        get => _innerLabel.FontColor;
        set => _innerLabel.FontColor = value;
    }

    /// <summary>
    /// Gets or sets the background visual for this button when it is being pressed.
    /// </summary>
    /// <remarks>
    /// Not setting this property will result in no change to the button's background when clicked: a typically undesirable behavior.
    /// </remarks>
    public IVisual? PressedBackground
    {  get; set;  }

    /// <summary>
    /// Gets or sets the visual for this button's border when it is being pressed.
    /// </summary>
    /// <remarks>
    /// Not setting this property will result in no change to the appearance of the button's border when clicked.
    /// clicked.
    /// </remarks>
    public IVisual? PressedBorder
    { get; set; }

    /// <summary>
    /// Gets or sets the visual component data for an optional image to display alongside the button's text.
    /// </summary>
    public IVisualRegion? Image
    {
        get => _innerImage.Visual;
        set => _innerImage.Visual = value;
    }

    /// <summary>
    /// Gets or sets the specific width of the image displayed alongside the button's text.
    /// </summary>
    public int? ImageWidth
    {
        get => _innerImage.Width;
        set => _innerImage.Width = value;
    }

    /// <summary>
    /// Gets or sets the specific height of the image displayed alongside the button's text.
    /// </summary>
    public int? ImageHeight
    {
        get => _innerImage.Height;
        set => _innerImage.Height = value;
    }

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
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        _innerPanel.Draw(spriteBatch);
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseButton pressedButton)
    {
        base.OnMouseDown(pressedButton);

        if (pressedButton == MouseButton.Left)
            _isPressed = true;
    }

    /// <inheritdoc/>
    protected override void OnMouseUp(MouseButton releasedButton)
    {
        base.OnMouseUp(releasedButton);

        if (releasedButton == MouseButton.Left)
        {
            _isPressed = false;

            // If the mouse button was released while the pointer was still over this control, then our button was clicked.
            if (IsMouseOver)
                Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBackground()
    {
        if (_isPressed)
        {
            if (IsMouseOver && PressedBackground != null)
                return PressedBackground;

            // We only show a pressed state if the button control is being pressed as the mouse is over it, otherwise
            // we display a hovered state. This behavior is consistent with that of other common Windows controls.
            if (!IsMouseOver && HoveredBackground != null)
                return HoveredBackground;
        }

        return base.GetActiveBackground();
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBorder()
    {   // The same logic applied to determining which background is active is also applied here.
        if (_isPressed)
        {
            if (IsMouseOver && PressedBorder != null)
                return PressedBorder;

            if (!IsMouseOver && HoveredBorder != null)
                return HoveredBorder;
        }

        return base.GetActiveBorder();
    }
}
