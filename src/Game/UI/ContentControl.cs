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

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a control with a single piece of content of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of content rendered by this control.</typeparam>
public abstract class ContentControl<T> : Control
    where T : Control, new()
{
    /// <inheritdoc/>
    public override IInputHandler? InputHandler
    {
        get => base.InputHandler; 
        internal set => base.InputHandler = Content.InputHandler = value;
    }

    /// <summary>
    /// Gets the content displayed by this control.
    /// </summary>
    public T Content 
    { get; } = new();

    /// <inheritdoc/>
    public override bool Focus() 
        // Content is giving focus, but we abort if this control cannot receive focus (i.e., due to not being focusable, etc.).
        => base.Focus() && Content.Focus();

    /// <inheritdoc/>
    public override void UpdateInput()
    {
        base.UpdateInput();

        Content.UpdateInput();
    }

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
    {
        Content.Measure(availableSize);

        return Content.DesiredSize;
    }

    /// <inheritdoc/>
    protected override void ArrangeCore()
    {
        base.ArrangeCore();

        Content.Arrange(ContentBounds);
    }

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
        => Content.Draw(spriteBatch);
}
