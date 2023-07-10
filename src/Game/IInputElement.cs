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

namespace BadEcho.Game;

/// <summary>
/// Defines an element that supports basic input processing.
/// </summary>
public interface IInputElement
{
    /// <summary>
    /// Gets a value indicating whether the mouse pointer is located over the element.
    /// </summary>
    bool IsMouseOver { get; }

    /// <summary>
    /// Gets a value indicating if the element is focusable, and therefore able to receive input from the keyboard.
    /// </summary>
    bool IsFocusable { get; }

    /// <summary>
    /// Gets a value indicating if the element has keyboard focus.
    /// </summary>
    bool IsFocused { get; }

    /// <summary>
    /// Gets a value indicating if key press events are repeated for every <c>Update</c> tick in which the key remains held down.
    /// </summary>
    bool NotifyKeyRepeats{ get; }

    /// <summary>
    /// Clears the keyboard focus from this element, preventing it from handling any further keyboard input.
    /// </summary>
    void ClearFocus();

    /// <summary>
    /// Attempts to set keyboard focus on this input element, allowing it to handle keyboard input.
    /// </summary>
    /// <returns>True if this element has keyboard focus; otherwise, false.</returns>
    bool Focus();
}
