﻿//-----------------------------------------------------------------------
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
/// <remarks>
/// <para>
/// The goal here isn't to provide an incredibly rich and over-engineered user interface framework, but rather the minimum
/// functionality required in order to be able to comfortably create interface elements for a game. Still, we do embrace some of
/// the approaches taken by some of the more fully fleshed out general-purpose user interface frameworks out there. 
/// </para>
/// <para>
/// Specifically, we adopt the notion of a two-part layout process consisting of <c>Measure</c> and <c>Arrange</c> passes that must
/// be executed if the layout has been invalidated prior to any actual rendering. This is a core concept a few user interface frameworks
/// such as Windows Presentation Foundation.
/// </para>
/// <para>
/// Other than that, the intention is to keep the foundational logic for controls powered by this framework as simple as practicable.
/// </para>
/// </remarks>
public abstract class Control : IArrangeable, IInputElement
{
    private readonly List<MouseButton> _pressedButtons = new();
    private readonly List<Keys> _pressedKeys = new();

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

    /// <summary>
    /// Gets the parent of this control.
    /// </summary>
    public IArrangeable? Parent
    { get; internal set; }

    /// <summary>
    /// Gets the source of user input for this control.
    /// </summary>
    public virtual IInputHandler? InputHandler
    { get; internal set; }

    /// <summary>
    /// Gets the desired size of this control.
    /// </summary>
    public Size DesiredSize
    { get; private set; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control, which includes its margin, border, background,
    /// padding, and content.
    /// </summary>
    public Rectangle LayoutBounds
    { get; protected set; }

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control's border, which includes its background, padding,
    /// and content.
    /// </summary>
    public Rectangle BorderBounds
        => Margin.ApplyMargin(LayoutBounds);

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control's background, padding, and content.
    /// </summary>
    public Rectangle BackgroundBounds
        => BorderThickness.ApplyMargin(BorderBounds);

    /// <summary>
    /// Gets the bounding rectangle of the region occupied by this control's padding and actual content.
    /// </summary>
    public Rectangle ContentBounds
        => Padding.ApplyMargin(BackgroundBounds);

    /// <summary>
    /// Gets or sets a value indicating if the control is enabled in the user interface and receiving input.
    /// </summary>
    public bool IsEnabled
    { get; internal set; } = true;

    /// <summary>
    /// Gets or sets a value indicating if this control is visible.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => RemeasureIfChanged(ref _isVisible, value);
    }

    /// <summary>
    /// Gets or sets the space between this control and other elements adjacent to it when the layout is generated.
    /// </summary>
    public Thickness Margin
    {
        get => _margin;
        set => RemeasureIfChanged(ref _margin, value);
    }

    /// <summary>
    /// Gets or sets the space between this control's border and its content.
    /// </summary>
    public Thickness Padding
    {
        get => _padding;
        set => RemeasureIfChanged(ref _padding, value);
    }

    /// <summary>
    /// Gets or sets the thickness of this control's border.
    /// </summary>
    public Thickness BorderThickness
    {
        get => _borderThickness;
        set => RemeasureIfChanged(ref _borderThickness, value);
    }

    /// <summary>
    /// Gets or sets the specific width of the control.
    /// </summary>
    /// <remarks>If a width is not explicitly set, then the control's width will be sized based on the size of its content.</remarks>
    public int? Width
    {
        get => _width;
        set => RemeasureIfChanged(ref _width, value);
    }

    /// <summary>
    /// Gets or sets the minimum width of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure the control's width is at least the specified value, taking precedence over <see cref="MaximumWidth"/> and
    /// <see cref="Width"/> values in regards to the minimum width constraint. If a minimum width is not set, then no minimum width
    /// constraint exists.
    /// </remarks>
    public int? MinimumWidth
    {
        get => _minimumWidth;
        set => RemeasureIfChanged(ref _minimumWidth, value);
    }

    /// <summary>
    /// Gets or sets the maximum width of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure the control's width doesn't exceed the specified value, however the <see cref="MinimumWidth"/> will take precedence
    /// over it if it happens to exceed this value. If a maximum width is not set, then no maximum width constraint exists.
    /// </remarks>
    public int? MaximumWidth
    {
        get => _maximumWidth;
        set => RemeasureIfChanged(ref _maximumWidth, value);
    }

    /// <summary>
    /// Gets or sets the specific height of the control.
    /// </summary>
    /// <remarks>If a height is not explicitly set, then the control's height will be sized based on the size of its content.</remarks>
    public int? Height
    {
        get => _height;
        set => RemeasureIfChanged(ref _height, value);
    }

    /// <summary>
    /// Gets or sets the minimum height of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure the control's height is at least the specified value, taking precedence over <see cref="MaximumHeight"/> and
    /// <see cref="Height"/> values in regards to the minimum height constraint. If a minimum height is not set, then no minimum height
    /// constraint exists.
    /// </remarks>
    public int? MinimumHeight
    {
        get => _minimumHeight;
        set => RemeasureIfChanged(ref _minimumHeight, value);
    }

    /// <summary>
    /// Gets or sets the maximum height of the control.
    /// </summary>
    /// <remarks>
    /// This will ensure the control's height doesn't exceed the specified value, however the <see cref="MinimumHeight"/> will take precedence
    /// over it if it happens to exceeds this value. If a maximum height is not set, then no maximum height constraint exists.
    /// </remarks>
    public int? MaximumHeight
    {
        get => _maximumHeight;
        set => RemeasureIfChanged(ref _maximumHeight, value);
    }

    /// <summary>
    /// Gets or sets the index of the column within a parent <see cref="Grid"/> panel containing this control.
    /// </summary>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    public int Column
    {
        get => _column;
        set => RemeasureIfChanged(ref _column, value);
    }

    /// <summary>
    /// Gets or sets the index of the row within a parent <see cref="Grid"/> panel containing this control.
    /// </summary>
    /// <remarks>
    /// This property is ignored if <see cref="Parent"/> is not set to a <see cref="Grid"/> instance.
    /// </remarks>
    public int Row
    {
        get => _row;
        set => RemeasureIfChanged(ref _row, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment characteristics that are applied to this control when composed in a layout parent, such as a
    /// type of <see cref="Panel"/>.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment
    {
        get => _horizontalAlignment;
        set => RearrangeIfChanged(ref _horizontalAlignment, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment characteristics that are applied to this control when composed in a layout parent, such as a type
    /// of <see cref="Panel"/>.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        set => RearrangeIfChanged(ref _verticalAlignment, value);
    }
    
    /// <summary>
    /// Gets or sets the background visual of this control.
    /// </summary>
    public IVisual? Background
    { get; set; }

    /// <summary>
    /// Gets or sets the background visual of this control when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    public IVisual? DisabledBackground
    { get; set; }

    /// <summary>
    /// Gets or sets the background visual of this control when the cursor is located over it.
    /// </summary>
    /// <remarks>
    /// Not setting this property will result in no change to the control's background when the cursor is over it.
    /// </remarks>
    public IVisual? HoveredBackground
    { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border.
    /// </summary>
    public IVisual? Border
    { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border when it has been disabled via the <see cref="IsEnabled"/> property.
    /// </summary>
    public IVisual? DisabledBorder
    { get; set; }

    /// <summary>
    /// Gets or sets the visual of the control's border when the cursor is located over it.
    /// </summary>
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
    /// The measurement state will be recalculated by this control's parent calling <see cref="Measure"/> during the next <c>Measure</c> pass.
    /// Calling this will also call <see cref="InvalidateArrange"/>; it should only be called if a size-altering change to the control has been
    /// made.
    /// </remarks>
    public void InvalidateMeasure()
    {
        _invalidMeasure = true;

        InvalidateArrange();
        Parent?.InvalidateMeasure();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The arrangement state will be recalculated by this control's parent calling <see cref="Arrange"/> during the next <c>Arrange</c> pass.
    /// </remarks>
    public void InvalidateArrange()
    {
        _invalidArrange = true;

        Parent?.InvalidateArrange();
    }

    /// <summary>
    /// Updates the desired size of this control, called during the <c>Measure</c> pass of a layout update.
    /// </summary>
    /// <param name="availableSize">The available space a parent control is allocating for this control.</param>
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

    /// <summary>
    /// Updates the position of this control, called during the <c>Arrange</c> pass of a layout update.
    /// </summary>
    /// <param name="effectiveArea">
    /// The effective area within the parent that this control should use to arrange itself and any children.   
    /// </param>
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

    /// <summary>
    /// Draws the control to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="ConfiguredSpriteBatch"/> instance to use to draw the control.</param>
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

    /// <summary>
    /// Processes events related to user input.
    /// </summary>
    /// <exception cref="InvalidOperationException">Control does not have a valid <see cref="InputHandler"/> assigned.</exception>
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
    /// regardless of whether or not the mouse is still over this control.
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
