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
/// Defines a user element that allows for the selection of one or more of its constituent parts. 
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Gets a value indicating if this element's items can be selected by the user.
    /// </summary>
    bool IsSelectable { get; }

    /// <summary>
    /// Gets or sets the background visual of an item when the cursor is located over it.
    /// </summary>
    IVisual? HoveredItemBackground { get; set; }

    /// <summary>
    /// Gets or sets the background visual of an item when it has been selected.
    /// </summary>
    IVisual? SelectedItemBackground { get; set; }
}
