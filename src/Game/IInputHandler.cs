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
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game;

/// <summary>
/// Defines a single source and management point for input provided by the user.
/// </summary>
public interface IInputHandler
{
    /// <summary>
    /// Gets the last captured position of the mouse by the handler.
    /// </summary>
    Point MousePosition { get; }

    /// <summary>
    /// Gets the mouse buttons currently being pressed by the user.
    /// </summary>
    IEnumerable<MouseButton> PressedButtons { get; }

    /// <summary>
    /// Gets the keyboard keys currently being pressed by the user.
    /// </summary>
    IEnumerable<Keys> PressedKeys { get; }

    /// <summary>
    /// Gets or sets the <see cref="IInputElement"/> instance that currently has keyboard focus.
    /// </summary>
    IInputElement? FocusedElement { get; set; }

    /// <summary>
    /// Clears the keyboard focus, preventing any keyboard input from being handled.
    /// </summary>
    void ClearFocus();
}
