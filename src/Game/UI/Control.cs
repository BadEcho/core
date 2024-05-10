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
using BadEcho.Extensions;
using BadEcho.Game.Properties;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a base class for user interface elements in a game.
/// </summary>
/// <typeparam name="TSelf">The type of control deriving from this base.</typeparam>
/// <remarks>
/// <para>
/// The goal here isn't to provide an incredibly rich and over-engineered user interface framework, but rather the
/// minimum functionality required in order to be able to comfortably create interface elements for a game.
/// Still, we do embrace some of the approaches taken by some of the more fully fleshed out general-purpose user
/// interface frameworks out there. 
/// </para>
/// <para>
/// Specifically, we adopt the notion of a two-part layout process consisting of <c>Measure</c> and <c>Arrange</c> passes
/// that must be executed if the layout has been invalidated prior to any actual rendering. This is a core concept shared
/// by a few user interface frameworks such as Windows Presentation Foundation.
/// </para>
/// <para>
/// Other than that, the intention is to keep the foundational logic for controls powered by this framework as
/// simple as practicable.
/// </para>
/// <para>
/// This type implements the "curiously recurring template pattern" in order to support declarative styling à la WPF
/// (albeit with a bit more type safety). Derived types should pass themselves as the generic type parameter.
/// </para>
/// </remarks>
public abstract class Control<TSelf> : IControl
    where TSelf : Control<TSelf>
{
    private readonly List<MouseButton> _pressedButtons = [];
    private readonly List<Keys> _pressedKeys = [];

    private bool _invalidArrange = true;
    private bool _invalidMeasure = true;
    private bool _isVisible = true;

    private int? _width;
    private int? _minimumWidth;
    private int? _maximumWidth;
    private int? _height;
    private int? _minimumHeight;
    private int? _maximumHeight;

    private int _column;
    private int _row;

    private HorizontalAlignment _horizontalAlignment;
    private VerticalAlignment _verticalAlignment;
    private Thickness _borderThickness;
    private Thickness _margin;
    private Thickness _padding;

    private Size _lastAvailableSize;
    private Rectangle _lastEffectiveArea;
    private Point? _lastMousePosition;

    private Style<TSelf>? _style;

    /// <summary>
    /// Gets the source of user input for this control.
    /// </summary>
    public virtual IInputHandler? InputHandler
    { get; set; }

    /// <inheritdoc/>
    public IArrangeable? Parent
    { get; set; }

    /// <inheritdoc/>
    public Size DesiredSize
    { get; private set; }

    /// <inheritdoc/>
    public Rectangle LayoutBounds
    { get; protected set; }

    /// <inheritdoc/>
    public Rectangle BorderBounds
        => Margin.ApplyMargin(LayoutBounds);

    /// <inheritdoc/>
    public Rectangle BackgroundBounds
        => BorderThickness.ApplyMargin(BorderBounds);

    /// <inheritdoc/>
    public Rectangle ContentBounds
        => Padding.ApplyMargin(BackgroundBounds);

    /// <inheritdoc/>
    public bool IsEnabled
    { get; internal set; } = true;

    /// <inheritdoc/>
    public bool IsVisible
    {
        get => _isVisible;
        set => RemeasureIfChanged(ref _isVisible, value);
    }

    /// <inheritdoc/>
    public Thickness Margin
    {
        get => _margin;
        set => RemeasureIfChanged(ref _margin, value);
    }

    /// <inheritdoc/>
    public Thickness Padding
    {
        get => _padding;
        set => RemeasureIfChanged(ref _padding, value);
    }

    /// <inheritdoc/>
    public Thickness BorderThickness
    {
        get => _borderThickness;
        set => RemeasureIfChanged(ref _borderThickness, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// If a width is not explicitly set, then this control's width will be sized based on the size of its content.
    /// </remarks>
    public int? Width
    {
        get => _width;
        set => RemeasureIfChanged(ref _width, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This will ensure this control's width is at least the specified value, taking precedence over <see cref="MaximumWidth"/>
    /// and <see cref="Width"/> values in regard to the minimum width constraint. If a minimum width is not set, then no
    /// minimum width constraint exists.
    /// </remarks>
    public int? MinimumWidth
    {
        get => _minimumWidth;
        set => RemeasureIfChanged(ref _minimumWidth, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This will ensure this control's width doesn't exceed the specified value, however the <see cref="MinimumWidth"/>
    /// will take precedence over it if it happens to exceed this value. If a maximum width is not set, then no maximum
    /// width constraint exists.
    /// </remarks>
    public int? MaximumWidth
    {
        get => _maximumWidth;
        set => RemeasureIfChanged(ref _maximumWidth, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// If a height is not explicitly set, then this control's height will be sized based on the size of its content.
    /// </remarks>
    public int? Height
    {
        get => _height;
        set => RemeasureIfChanged(ref _height, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This will ensure this control's height is at least the specified value, taking precedence over <see cref="MaximumHeight"/>
    /// and <see cref="Height"/> values in regard to the minimum height constraint. If a minimum height is not set, then no
    /// minimum height constraint exists.
    /// </remarks>
    public int? MinimumHeight
    {
        get => _minimumHeight;
        set => RemeasureIfChanged(ref _minimumHeight, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This will ensure this control's height doesn't exceed the specified value, however the <see cref="MinimumHeight"/>
    /// will take precedence over it if it happens to exceed this value. If a maximum height is not set, then no maximum
    /// height constraint exists.
    /// </remarks>
    public int? MaximumHeight
    {
        get => _maximumHeight;
        set => RemeasureIfChanged(ref _maximumHeight, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    public int Column
    {
        get => _column;
        set => RemeasureIfChanged(ref _column, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    public int Row
    {
        get => _row;
        set => RemeasureIfChanged(ref _row, value);
    }

    /// <inheritdoc/>
    public HorizontalAlignment HorizontalAlignment
    {
        get => _horizontalAlignment;
        set => RearrangeIfChanged(ref _horizontalAlignment, value);
    }

    /// <inheritdoc/>
    public VerticalAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        set => RearrangeIfChanged(ref _verticalAlignment, value);
    }
    
    /// <inheritdoc/>
    public IVisual? Background
    { get; set; }

    /// <inheritdoc/>
    public IVisual? DisabledBackground
    { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Not setting this property will result in no change to the control's background when the cursor is over it.
    /// </remarks>
    public IVisual? HoveredBackground
    { get; set; }

    /// <inheritdoc/>
    public IVisual? Border
    { get; set; }

    /// <inheritdoc/>
    public IVisual? DisabledBorder
    { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Not setting this property will result in no change to the appearance of the button's border when the cursor is over it.
    /// </remarks>
    public IVisual? HoveredBorder
    { get; set; }

    /// <inheritdoc/>
    public bool IsMouseOver
    { get; private set; }

    /// <inheritdoc/>
    public bool NotifyKeyRepeats
    { get; protected set; }

    /// <inheritdoc/>
    public bool IsFocusable
    { get; set; }

    /// <summary>
    /// Gets or sets the style used by this control when it is rendered.
    /// </summary>
    public Style<TSelf>? Style
    {
        get => _style;
        set
        {
            _style = value;

            if (_style == null)
                return;

            if (this is not TSelf self)
                throw new InvalidOperationException(Strings.ControlNotSelfRecurring);

            value?.ApplyTo(self);
            _style = value;
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(InputHandler))]
    public bool IsFocused
        => InputHandler != null && InputHandler.FocusedElement == this;

    /// <inheritdoc/>
    public void ClearFocus()
    {
        if (IsFocused)
            InputHandler.FocusedElement = null;
    }

    /// <inheritdoc/>
    public virtual bool Focus()
    {
        if (!IsFocusable || InputHandler == null)
            return false;

        InputHandler.FocusedElement = this;

        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The measurement state will be recalculated by this control's parent calling <see cref="Measure"/> during the next
    /// <c>Measure</c> pass. Calling this will also call <see cref="InvalidateArrange"/>; it should only be called if a
    /// size-altering change to the control has been made.
    /// </remarks>
    public void InvalidateMeasure()
    {
        _invalidMeasure = true;

        InvalidateArrange();
        Parent?.InvalidateMeasure();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The arrangement state will be recalculated by this control's parent calling <see cref="Arrange"/> during the next
    /// <c>Arrange</c> pass.
    /// </remarks>
    public void InvalidateArrange()
    {
        _invalidArrange = true;

        Parent?.InvalidateArrange();
    }

    /// <inheritdoc/>
    public void Measure(Size availableSize)
    {
        // Even if measurement isn't invalidated, we re-measure if the available space has changed.
        if (!_invalidMeasure && _lastAvailableSize == availableSize)
            return;

        _lastAvailableSize = availableSize;
        var contentMargin = Margin + BorderThickness + Padding;

        // The non-content area is added on to the measured size, it should not be part of the content's measurement calculation.
        Size availableSizeForContent = contentMargin.ApplyMargin(availableSize);
        Size desiredSize = contentMargin.ApplyPadding(MeasureCore(availableSizeForContent));

        // We disregard the content-based sizing for the width and height if they're set explicitly.
        int width = Width ?? desiredSize.Width;
        int height = Height ?? desiredSize.Height;
        
        // Commit the newly measured size with size constraints enforced.
        DesiredSize = new Size(Math.Max(MinimumWidth ?? 0, Math.Min(MaximumWidth ?? width, width)),
                               Math.Max(MinimumHeight ?? 0, Math.Min(MaximumHeight ?? height, height)));

        _invalidMeasure = false;
    }

    /// <inheritdoc/>
    public void Arrange(Rectangle effectiveArea)
    {   // Even if the layout hasn't been invalidated, we will update the control's position if the effective area has changed.
        if (!_invalidArrange && _lastEffectiveArea == effectiveArea)
            return;

        _lastEffectiveArea = effectiveArea;

        // Regardless of what we've measured, we cannot exceed the parent's specified effective area.
        var desiredSize = new Size(Math.Min(DesiredSize.Width, effectiveArea.Width),
                                   Math.Min(DesiredSize.Height, effectiveArea.Height));
        
        Rectangle alignedArea = Align(effectiveArea.Size, desiredSize);

        alignedArea.Offset(effectiveArea.Location);
        LayoutBounds = alignedArea;
        
        ArrangeCore();

        _invalidArrange = false;
    }

    /// <inheritdoc/>
    public void Draw(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));
        
        if (!IsVisible)
            return;

        IVisual? background = GetActiveBackground();

        background?.Draw(spriteBatch, BackgroundBounds);

        IVisual? border = GetActiveBorder();

        if (border != null)
        {
            if (BorderThickness.Left > 0)
            {
                border.Draw(spriteBatch,
                            new Rectangle(BorderBounds.X, BorderBounds.Y, BorderThickness.Left, BorderBounds.Height));
            }

            if (BorderThickness.Top > 0)
            {
                border.Draw(spriteBatch,
                            new Rectangle(BorderBounds.X, BorderBounds.Y, BorderBounds.Width, BorderThickness.Top));
            }

            if (BorderThickness.Right > 0)
            {
                border.Draw(spriteBatch,
                            new Rectangle(BorderBounds.Right - BorderThickness.Right, BorderBounds.Y, BorderThickness.Right, BorderBounds.Height));
            }

            if (BorderThickness.Bottom > 0)
            {
                border.Draw(spriteBatch, new Rectangle(BorderBounds.X,
                                                       BorderBounds.Bottom - BorderThickness.Bottom,
                                                       BorderBounds.Width,
                                                       BorderThickness.Bottom));
            }
        }

        Rectangle clippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

        spriteBatch.GraphicsDevice.ScissorRectangle = ContentBounds;

        DrawCore(spriteBatch);

        spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;
    }

    /// <inheritdoc/>
    public virtual void UpdateInput()
    {
        if (InputHandler == null)
            throw new InvalidOperationException(Strings.NoInputHandler);

        UpdateMouseInput(InputHandler.MousePosition, InputHandler.PressedButtons.ToList());
        UpdateKeyboardInput(InputHandler.PressedKeys.ToList());
    }

    /// <summary>
    /// Called when the mouse pointer enters the boundaries of this control.
    /// </summary>
    protected virtual void OnMouseEnter()
    { }

    /// <summary>
    /// Called when the mouse pointer moves while over this control.
    /// </summary>
    protected virtual void OnMouseMove()
    { }

    /// <summary>
    /// Called when the mouse pointer leaves the boundaries of this control.
    /// </summary>
    protected virtual void OnMouseLeave()
    { }

    /// <summary>
    /// Called when a mouse button has been pressed while the mouse is over this control.
    /// </summary>
    /// <param name="pressedButton">The button of the mouse that has been pressed.</param>
    protected virtual void OnMouseDown(MouseButton pressedButton)
    { }

    /// <summary>
    /// Called when a mouse button, previously pressed while the mouse was over this control, has been released.
    /// </summary>
    /// <param name="releasedButton">The button of the mouse that has been released.</param>
    /// <remarks>
    /// This will always be called when a button (previously pressed while the mouse was over this control) has been released,
    /// regardless of whether the mouse is still over this control or not.
    /// </remarks>
    protected virtual void OnMouseUp(MouseButton releasedButton)
    { }

    /// <summary>
    /// Called when a keyboard key has been pressed while this control has keyboard focus.
    /// </summary>
    /// <param name="pressedKey">The key of the keyboard that has been pressed.</param>
    protected virtual void OnKeyDown(Keys pressedKey)
    { }

    /// <summary>
    /// Called when a keyboard key has been released.
    /// </summary>
    /// <param name="releasedKey">The key of the keyboard that has been released.</param>
    protected virtual void OnKeyUp(Keys releasedKey)
    { }

    /// <summary>
    /// Sets a property's backing field to the provided value, invalidating the measurement state if a change in value occurred.
    /// </summary>
    /// <typeparam name="T">The property value's type.</typeparam>
    /// <param name="field">A reference to the backing field for the property.</param>
    /// <param name="value">The value being assigned.</param>
    protected void RemeasureIfChanged<T>(ref T field, T value)
    {
        if (field.Equals<T>(value))
            return;

        field = value;

        InvalidateMeasure();
    }

    /// <summary>
    /// Sets a property's backing field to the provided value, invalidating the arrangement state if a change in value occurred.
    /// </summary>
    /// <typeparam name="T">The property value's type.</typeparam>
    /// <param name="field">A reference to the backing field for the property.</param>
    /// <param name="value">The value being assigned.</param>
    protected void RearrangeIfChanged<T>(ref T field, T value)
    {
        if (field.Equals<T>(value))
            return;

        field = value;

        InvalidateArrange();
    }

    private Rectangle Align(Size effectiveSize, Size desiredSize)
    {
        int alignedWidth = desiredSize.Width;

        // Explicitly set width values take precedence over stretch alignments, unless there's not enough room.
        if (HorizontalAlignment == HorizontalAlignment.Stretch)
            alignedWidth = Width.HasValue ? Math.Min(Width.Value, effectiveSize.Width) : effectiveSize.Width;

        int alignedHeight = desiredSize.Height;

        // Explicitly set height values take precedence over stretch alignments, unless there's not enough room.
        if (VerticalAlignment == VerticalAlignment.Stretch)
            alignedHeight = Height.HasValue ? Math.Min(Height.Value, effectiveSize.Height) : effectiveSize.Height;
            
        int alignedX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => (effectiveSize.Width - desiredSize.Width) / 2,
            HorizontalAlignment.Right => effectiveSize.Width - desiredSize.Width,
            _ => 0
        };

        int alignedY = VerticalAlignment switch
        {
            VerticalAlignment.Center => (effectiveSize.Height - desiredSize.Height) / 2,
            VerticalAlignment.Bottom => effectiveSize.Height - desiredSize.Height,
            _ => 0
        };

        return new Rectangle(alignedX, alignedY, alignedWidth, alignedHeight);
    }

    /// <summary>
    /// When overridden in a derived class, provides custom measurement logic for sizing this control.
    /// </summary>
    /// <param name="availableSize">The available space a parent control is allocating for this control.</param>
    /// <returns>The desired size of this control.</returns>
    protected abstract Size MeasureCore(Size availableSize);

    /// <summary>
    /// When overridden in a derived class, provides custom arrangement logic for the positioning of this control.
    /// </summary>
    /// <remarks>
    /// By the time this is called, <see cref="LayoutBounds"/> and its associated properties will have already been set,
    /// and can be referenced by a derived class to determine its positioning.
    /// </remarks>
    protected virtual void ArrangeCore()
    { }

    /// <summary>
    /// Executes the custom rendering logic required to draw the control to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="ConfiguredSpriteBatch"/> instance to use to draw the control.</param>
    /// <remarks>
    /// This is where the actually rendering logic for the content (something this base type knows nothing about) needs to occur.
    /// </remarks>
    protected abstract void DrawCore(ConfiguredSpriteBatch spriteBatch);

    /// <summary>
    /// When overridden in a derived class, provides custom logic for selecting the background visual of the control based on its
    /// state.
    /// </summary>
    /// <returns>The background visual for the control.</returns>
    protected virtual IVisual? GetActiveBackground()
    {
        if (!IsEnabled)
            return DisabledBackground ?? Background;

        return IsMouseOver && HoveredBackground != null
            ? HoveredBackground
            : Background;
    }

    /// <summary>
    /// When overridden in a derived class, provides custom logic for selecting the visual of the control's border.
    /// </summary>
    /// <returns>The visual for the control's border.</returns>
    protected virtual IVisual? GetActiveBorder()
    {
        if (!IsEnabled)
            return DisabledBorder ?? Border;

        return IsMouseOver && HoveredBorder != null
            ? HoveredBorder
            : Border;
    }

    private void UpdateMouseInput(Point mousePosition, IReadOnlyCollection<MouseButton> pressedButtons)
    {
        IsMouseOver = BorderBounds.Contains(mousePosition);

        if (IsMouseOver)
        {
            if (_lastMousePosition == null)
                OnMouseEnter();

            if (_lastMousePosition != mousePosition)
                OnMouseMove();

            foreach (MouseButton pressedButton in pressedButtons)
            {
                if (!_pressedButtons.Contains(pressedButton))
                    _pressedButtons.Add(pressedButton);

                OnMouseDown(pressedButton);
            }

            _lastMousePosition = mousePosition;
        }
        else
        {
            if (_lastMousePosition != null)
                OnMouseLeave();

            _lastMousePosition = null;
        }

        IEnumerable<MouseButton> releasedButtons = _pressedButtons.Except(pressedButtons)
                                                                  .ToList();
        foreach (MouseButton releasedButton in releasedButtons)
        {
            _pressedButtons.Remove(releasedButton);

            OnMouseUp(releasedButton);
        }
    }

    private void UpdateKeyboardInput(IReadOnlyCollection<Keys> pressedKeys)
    {
        if (!IsFocused)
            return;

        foreach (Keys pressedKey in pressedKeys)
        {
            bool isRepeat = _pressedKeys.Contains(pressedKey);

            if (!isRepeat)
            {
                _pressedKeys.Add(pressedKey);
                OnKeyDown(pressedKey);
            }
            else if (NotifyKeyRepeats)
                OnKeyDown(pressedKey);
        }

        IEnumerable<Keys> releasedKeys = _pressedKeys.Except(pressedKeys)
                                                     .ToList();
        foreach (Keys releasedKey in releasedKeys)
        {
            _pressedKeys.Remove(releasedKey);

            OnKeyUp(releasedKey);
        }
    }
}
