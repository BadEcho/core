//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Provides an item belonging to a menu.
/// </summary>
public sealed class MenuItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="id">The identifier that was used to register the menu item with the menu.</param>
    /// <param name="label">The text of the menu item.</param>
    internal MenuItem(int id, string label)
    {
        Id = id;
        Label = label;
    }
    
    /// <summary>
    /// Gets the identifier that was used to register the menu item with the menu.
    /// </summary>
    public int Id
    { get; }

    /// <summary>
    /// Gets the text of the menu item.
    /// </summary>
    public string Label 
    { get; }
}
