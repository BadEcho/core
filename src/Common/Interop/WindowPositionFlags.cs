// -----------------------------------------------------------------------
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
/// Specifies flags that control the sizing and positioning of a window.
/// </summary>
[Flags]
internal enum WindowPositionFlags : uint
{
    /// <summary>
    /// Retain the current size.
    /// </summary>
    NoSize = 0x1,
    /// <summary>
    /// Retain the current position.
    /// </summary>
    NoMove = 0x2,
    /// <summary>
    /// Retain the current Z order.
    /// </summary>
    NoZOrder = 0x4,
    /// <summary>
    /// Do not redraw changes.
    /// </summary>
    NoRedraw = 0x8,
    /// <summary>
    /// Do not activate the window.
    /// </summary>
    NoActivate = 0x10,
    /// <summary>
    /// Apply new frame styles set using the <see cref="User32.SetWindowLongPtr"/> function.
    /// </summary>
    FrameChanged = 0x20,
    /// <summary>
    /// Display the window.
    /// </summary>
    ShowWindow = 0x40,
    /// <summary>
    /// Hide the window.
    /// </summary>
    HideWindow = 0x80,
    /// <summary>
    /// Discards the entire contents of the client area.
    /// </summary>
    DiscardClientArea = 0x100,
    /// <summary>
    /// Retain the owner window's current Z order.
    /// </summary>
    NoOwnerZOrder = 0x200,
    /// <summary>
    /// Prevent the window from receiving the <see cref="WindowMessage.WindowPositionChanging"/> message.
    /// </summary>
    NoSendChanging = 0x400,
    /// <summary>
    /// Prevent generation of the <see cref="WindowMessage.SyncPaint"/> message.
    /// </summary>
    DeferErase = 0x2000,
    /// <summary>
    /// Post the request to the thread that owns the window if the calling thread and thread that owns said window are attached
    /// to different input queues.
    /// </summary>
    AsyncWindowPosition = 0x4000
}
