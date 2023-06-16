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
        
        if (IsMouseOver)
        {
            // We independently track whether the mouse button releases from presses.
            // This is done for purposes of maintaining consistency with common Windows controls, which exhibit the behavior of
            // reverting to a "mouse-over" appearance if the mouse pointer leaves the boundaries of a button with the left button depressed.
            _isReleased
                = mouseState.LeftButton == ButtonState.Released;

            if (_isReleased && _isPressed)
            {   // The button was pressed and has been released while the mouse pointer was over our button, thereby constituting a button click.
                Clicked?.Invoke(this, EventArgs.Empty);
            }

            _isPressed = mouseState.LeftButton == ButtonState.Pressed;
        }
        else
        {   // Reset the pressed state if the mouse is no longer over the control, the prevent a click occurring from releasing the button and then 
            // moving the mouse back over the button.
            _isPressed = false;
        }
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBackground()
    {
        if (_isPressed && PressedBackground != null)
            return PressedBackground;

        // Check if the button has not been released since being pressed while the mouse was over the button -- we only show a pressed state if a button
        // is pressed while over the button, otherwise we take one step back to a "mouse-over" appearance.
        if (!_isReleased && HoveredBackground != null)
            return HoveredBackground;

        return base.GetActiveBackground();
    }

    /// <inheritdoc/>
    protected override IVisual? GetActiveBorder()
    {
        if (_isPressed && PressedBorder != null)
            return PressedBorder;

        // Like with the background, if the button hasn't been released, yet our mouse is no longer over the button, we revert from a "pressed" appearance
        // to a "mouse-over" appearance.
        if (!_isReleased && HoveredBorder != null)
            return HoveredBorder;

        return base.GetActiveBorder();
    }
}
