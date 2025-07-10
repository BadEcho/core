﻿// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies a type of standard message value used when sending or posting messages to windows.
/// </summary>
public enum WindowMessage
{
    /// <summary>
    /// A window message corresponding to WM_NULL, posted when...absolutely nothing is happening.
    /// </summary>
    Null = 0,
    /// <summary>
    /// A window message corresponding to a window being destroyed (WM_DESTROY).
    /// </summary>
    Destroy = 0x2,
    /// <summary>
    /// A window message corresponding to a window being resized (WM_SIZE).
    /// </summary>
    Size = 0x5,
    /// <summary>
    /// A window message corresponding to WM_CLOSE.
    /// </summary>
    Close = 0x10,
    /// <summary>
    /// A window message corresponding to WM_ERASEBKGND, posted when the window background must be erased.
    /// </summary>
    EraseBackground = 0x14,
    /// <summary>
    /// A window message corresponding to WM_SHOWWINDOW, posted when the window is about to be hidden or shown.
    /// </summary>
    ShowWindow = 0x18,
    /// <summary>
    /// A window message corresponding to WM_WINDOWPOSCHANGING, posted when the window's size, position, or place in the
    /// Z order is about to change.
    /// </summary>
    WindowPositionChanging = 0x46,
    /// <summary>
    /// A window message corresponding to a nonclient area being created (WM_NCCREATE).
    /// </summary>
    CreateNonclientArea = 0x81,
    /// <summary>
    /// A window message corresponding to a nonclient area being destroyed (WM_NCDESTROY).
    /// </summary>
    DestroyNonclientArea = 0x82,
    /// <summary>
    /// A window message corresponding to WM_SYNCPAINT, used to synchronize painting while avoiding linking independent GUI threads.
    /// </summary>
    SyncPaint = 0x88,
    /// <summary>
    /// A window message corresponding to a nonsystem key being pressed (WM_KEYDOWN). A nonsystem key is a key that is pressed
    /// when the ALT key is not pressed.
    /// </summary>
    KeyDown = 0x100,
    /// <summary>
    /// A window message corresponding to WM_LBUTTONUP, posted when the user releases the left mouse button while the cursor
    /// is in the client area of a window.
    /// </summary>
    LeftButtonUp = 0x202,
    /// <summary>
    /// A window message corresponding to WM_RBUTTONUP, posted when the user releases the right mouse button while the cursor
    /// is in the client area of a window.
    /// </summary>
    RightButtonUp = 0x205,
    /// <summary>
    /// A window message corresponding to WM_DRAWCLIPBOARD, sent to windows in the clipboard viewer chain when the content of
    /// the clipboard changes.
    /// </summary>
    DrawClipboard = 0x308,
    /// <summary>
    /// A window message corresponding to WM_CHANGECBCHAIN, sent to windows in the clipboard viewer chain when a window is being
    /// removed from the chain.
    /// </summary>
    ChangeClipboardChain = 0x30D,
    /// <summary>
    /// A window message corresponding to WM_HOTKEY, indicating a hot key has been pressed.
    /// </summary>
    HotKey = 0x312,
    /// <summary>
    /// A window message corresponding to WM_CLIPBOARDUPDATE, indicating the contents of the clipboard have changed.
    /// </summary>
    ClipboardUpdate = 0x31D,
    /// <summary>
    /// A window message corresponding to WM_USER, used as a basis for messages for private window classes.
    /// </summary>
    User = 0x400
}