// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

namespace BadEcho.Hooks;

/// <summary>
/// Specifies a mouse input event.
/// </summary>
public enum MouseEvent
{
    /// <summary>
    /// The left button has been pressed.
    /// </summary>
    LeftButtonDown,
    /// <summary>
    /// The left button has been released.
    /// </summary>
    LeftButtonUp,
    /// <summary>
    /// The right button has been pressed.
    /// </summary>
    RightButtonDown,
    /// <summary>
    /// The right button has been released.
    /// </summary>
    RightButtonUp,
    /// <summary>
    /// The mouse cursor moved.
    /// </summary>
    Move,
    /// <summary>
    /// The scroll wheel has been rotated.
    /// </summary>
    Wheel,
    /// <summary>
    /// The horizontal scroll wheel has been rotated or tilted.
    /// </summary>
    HorizontalWheel,
    /// <summary>
    /// The middle button has been pressed.
    /// </summary>
    MiddleButtonDown,
    /// <summary>
    /// The middle button has been released.
    /// </summary>
    MiddleButtonUp,
    /// <summary>
    /// An extended mouse button has been pressed.
    /// </summary>
    XButtonDown,
    /// <summary>
    /// An extended mouse button has been released. 
    /// </summary>
    XButtonUp
}
