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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a rendering surface for drawing user interface elements.
/// </summary>
/// <remarks>
/// <para>
/// In order to properly render user interface elements, they must be associated with a <see cref="Screen"/> instance, which can
/// host any number of <see cref="Control"/> elements belonging to the root layout <see cref="Panel"/> instance assigned to it.
/// </para>
/// <para>
/// Although user interface elements can be technically drawn without a <see cref="Screen"/> instance, this class takes care of
/// arranging all necessary <c>Measure</c> and <c>Arrange</c> passes in a manner that synchronizes with the game's <c>Update</c>
/// and <c>Draw</c> cycles.
/// </para>
/// </remarks>
public sealed class Screen : IArrangeable, IInputHandler
{
    private readonly List<MouseButton> _pressedButtons = new();
    private readonly List<Keys> _pressedKeys = new();

    private readonly GraphicsDevice _device;

    private KeyboardState _currentKeyboardState;
    private MouseState _currentMouseState;
    private Rectangle _screenBounds;
    private Panel _content;

    private bool _invalidArrange = true;
    private bool _invalidMeasure = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="device">The graphics device to derive viewport information (i.e., screen bounds) from.</param>
    public Screen(GraphicsDevice device)
        : this(device, new StackPanel())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="device">The graphics device to derive viewport information (i.e., screen bounds) from.</param>
    /// <param name="content">The root layout panel to render on the surface.</param>
    public Screen(GraphicsDevice device, Panel content)
    {
        Require.NotNull(device, nameof(device));
        Require.NotNull(content, nameof(content));

        _device = device;
        _screenBounds = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);

        _content = content;
        _content.Parent = this;
        _content.InputHandler = this;
    }

    /// <summary>
    /// Gets or sets the root layout panel to render on this surface.
    /// </summary>
    public Panel Content
    {
        get => _content;
        set
        {
            if (value == _content)
                return;
            
            _content.Parent = null;
            _content.InputHandler = null;
            _content = value;
            _content.Parent = this;
            _content.InputHandler = this;
        }
    }

    /// <summary>
    /// Gets the coordinates to the upper-left corner of this surface.
    /// </summary>
    public Point Origin
    { get; set; }

    /// <inheritdoc/>
    public Point MousePosition
        => _currentMouseState.Position;

    /// <inheritdoc/>
    public IEnumerable<MouseButton> PressedButtons
        => _pressedButtons;

    /// <inheritdoc/>
    public IEnumerable<Keys> PressedKeys
        => _pressedKeys;

    /// <inheritdoc/>
    public IInputElement? FocusedElement 
    { get; set; }

    /// <inheritdoc/> 
    public void ClearFocus()
        => FocusedElement = null;

    /// <inheritdoc />
    public void InvalidateMeasure()
    {
        _invalidMeasure = true;

        InvalidateArrange();
    }

    /// <inheritdoc />
    public void InvalidateArrange()
    {
        _invalidArrange = true;
    }

    /// <summary>
    /// Performs any necessary updates to the user interface's layout.
    /// </summary>
    public void Update()
    {
        var screenBounds = new Rectangle(Origin.X, Origin.Y, _device.Viewport.Width, _device.Viewport.Height);

        if (screenBounds != _screenBounds)
            InvalidateMeasure();

        if (_invalidMeasure)
        {
            Content.Measure(screenBounds.Size);
            _invalidMeasure = false;
        }

        if (_invalidArrange)
        {
            Content.Arrange(screenBounds);
            _invalidArrange = false;
        }

        _screenBounds = screenBounds;

        UpdateInput();
    }

    /// <summary>
    /// Draws the user interface to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="ConfiguredSpriteBatch"/> instance to use to draw the user interface.</param>
    public void Draw(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        Content.Draw(spriteBatch);
    }

    private void UpdateInput()
    {
        _currentKeyboardState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();

        _pressedKeys.Clear();
        _pressedKeys.AddRange(_currentKeyboardState.GetPressedKeys());

        _pressedButtons.Clear();
        _pressedButtons.AddRange(_currentMouseState.GetPressedButtons());

        Content.UpdateInput();
    }
}
