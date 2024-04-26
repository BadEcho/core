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
/// Provides a base class for a layout parent control, responsible for positioning child controls on a rendering surface or
/// parent panel control.
/// </summary>
public abstract class Panel : Control
{
    private readonly List<Control> _children = [];

    /// <inheritdoc/>
    public override IInputHandler? InputHandler
    {
        get => base.InputHandler;
        internal set
        {
            if (InputHandler != value) 
                _children.ForEach(c => c.InputHandler = value);

            base.InputHandler = value;
        }
    }

    /// <summary>
    /// Gets a collection of all child controls of this panel.
    /// </summary>
    public IReadOnlyCollection<Control> Children
        => _children;

    /// <summary>
    /// Adds the provided control to this panel.
    /// </summary>
    /// <param name="child">The control to add to this panel.</param>
    public void AddChild(Control child)
    {
        Require.NotNull(child, nameof(child));

        _children.Add(child);

        child.Parent = this;
        InvalidateMeasure();
    }

    /// <summary>
    /// Removes the specified child control from this panel.
    /// </summary>
    /// <param name="child">The control to remove from this panel.</param>
    public void RemoveChild(Control child)
    {
        Require.NotNull(child, nameof(child));

        if (_children.Remove(child))
        {
            child.Parent = null;
            InvalidateMeasure();
        }
    }

    /// <inheritdoc/>
    public override void UpdateInput()
    {
        base.UpdateInput();

        IEnumerable<Control> activeChildren = Children.Where(c => c.IsEnabled);

        foreach (var activeChild in activeChildren)
        {
            activeChild.UpdateInput();
        }
    }

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        foreach (Control control in Children.Where(c => c.IsVisible))
        {
            control.Draw(spriteBatch);
        }
    }
}
