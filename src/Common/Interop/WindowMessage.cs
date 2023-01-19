//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies a type of standard message value used when sending or posting messages to windows.
/// </summary>
public enum WindowMessage
{
    /// <summary>
    /// A window message corresponding to WM_NULL.
    /// </summary>
    Null = 0,
    /// <summary>
    /// A window message corresponding to a window being destroyed (WM_DESTROY).
    /// </summary>
    Destroy = 0x2,
    /// <summary>
    /// A window message corresponding to WM_CLOSE.
    /// </summary>
    Close = 0x10,
    /// <summary>
    /// A window message corresponding to a nonclient area being created.
    /// </summary>
    CreateNonclientArea = 0x81,
    /// <summary>
    /// A window message corresponding to a nonclient area being destroyed.
    /// </summary>
    DestroyNonclientArea = 0x82,
    /// <summary>
    /// A window message corresponding to a nonsystem key being pressed. A nonsystem key is a key that is pressed
    /// when the ALT key is not pressed.
    /// </summary>
    KeyDown = 0x100,
    /// <summary>
    /// A window message indicating a hot key has been pressed, corresponding to WM_HOTKEY.
    /// </summary>
    HotKey = 0x312
}