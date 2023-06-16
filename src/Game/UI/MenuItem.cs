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
/// Provides a selectable item inside a <see cref="Menu"/> control.
/// </summary>
public sealed class MenuItem
{
    private readonly Menu _parent;

    private string? _label;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="parent">The menu containing the menu item.</param>
    public MenuItem(Menu parent)
    {
        Require.NotNull(parent, nameof(parent));

        _parent = parent;
    }

    /// <summary>
    /// Gets or sets the text of the menu item.
    /// </summary>
    public string? Label
    {
        get => _label;
        set
        {
            if (_label == value)
                return;

            _label = value;
            _parent.InvalidateMeasure();
        }
    }
}
