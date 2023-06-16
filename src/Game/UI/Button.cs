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
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a button user interface element, which can be clicked.
/// </summary>
public sealed class Button : Control
{
    private readonly Label _innerLabel;
    private readonly Image _innerImage;
    private readonly StackPanel _innerPanel;

    private bool _wasPressed;
    private bool _isPressed;
    private bool _isReleased;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    public Button()
    {
        _innerLabel = new Label
                      {
                          HorizontalAlignment = HorizontalAlignment.Center,
                          VerticalAlignment = VerticalAlignment.Center,
                          Parent = this
                      };

        _innerImage = new Image
                      {
                          HorizontalAlignment = HorizontalAlignment.Center,
                          VerticalAlignment = VerticalAlignment.Center,
                          Parent = this
                      };

        _innerPanel = new StackPanel
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
    public SpriteFont? Font
    {
        get => _innerLabel.Font;
        set => _innerLabel.Font = value;
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
    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        _innerPanel.Draw(spriteBatch);

        MouseState mouseState = Mouse.GetState();

        // Tracking both pressed and released states is needed in order to determine if an actually button click has occurred.
        _isReleased
            = mouseState.LeftButton == ButtonState.Released;

        if (IsMouseOver)
        {
            if (_isReleased && _isPressed)
            {   // The button was pressed and has been released while the mouse pointer was over our button, thereby constituting a button click.
                Clicked?.Invoke(this, EventArgs.Empty);
            }

            // We need to know if the mouse's button was pressed, in case the mouse moves away from it before its released.
            _wasPressed = _isPressed = mouseState.LeftButton == ButtonState.Pressed;
        }
        else
        {   // Reset the pressed state if the mouse is no longer over the control. The mouse must be over the button for it be considered "pressed",
            // regardless of whether the mouse's own buttons are pressed.
            _isPressed = false;

            if (_isReleased)
                _wasPressed = false;
        }
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBackground()
    {
        if (IsMouseOver && !_isPressed && HoveredBackground != null)
            return HoveredBackground;

        // Check if the button has not been released since being pressed while the mouse was over it -- we only show a pressed state if the button
        // is being pressed as the mouse is over it, otherwise we display a hovered state.
        // This behavior is consistent with that of other common Windows controls.
        if (!IsMouseOver && _wasPressed && !_isReleased && HoveredBackground != null)
            return HoveredBackground;

        if (_isPressed && PressedBackground != null)
            return PressedBackground;

        return base.GetActiveBackground();
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBorder()
    {
        if (IsMouseOver && !_isPressed && HoveredBorder != null)
            return HoveredBorder;

        // The same logic applied to determining which background is active is also  applied here.
        if (!IsMouseOver && _wasPressed && !_isReleased && HoveredBorder != null)
            return HoveredBorder;

        if (_isPressed && PressedBorder != null)
            return PressedBorder;

        return base.GetActiveBorder();
    }
}
