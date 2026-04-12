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
/// Specifies how a window is to be shown.
/// </summary>
internal enum ShowWindowCommand
{
    /// <summary>
    /// Hides the window and activates another window.
    /// </summary>
    Hide,
    /// <summary>
    /// Activates and displays a window. If the window is minimized, maximized, or arranged,
    /// the system restores it to its original size and position. Use this when showing a window for the first time.
    /// </summary>
    ShowNormal,
    /// <summary>
    /// Activates the window and displays it as a minimized window.
    /// </summary>
    ShowMinimized,
    /// <summary>
    /// Activates the window and displays it as a maximized window.
    /// </summary>
    ShowMaximized,
    /// <summary>
    /// Displays a window in its most recent size and position without activating it.
    /// </summary>
    ShowNormalNoActivate,
    /// <summary>
    /// Activates the window and displays it in its current size and position.
    /// </summary>
    Show,
    /// <summary>
    /// Minimizes the specified window and activates the next top-level window in the Z order.
    /// </summary>
    Minimize,
    /// <summary>
    /// Displays the window as a minimized window without activating it.
    /// </summary>
    ShowMinimizedNoActivate,
    /// <summary>
    /// Displays the window in its current size and position without activating it.
    /// </summary>
    ShowNoActivate,
    /// <summary>
    /// Activates and displays the window. If the window is minimized, maximized, or arranged,
    /// the system restores it to its original size and position.
    /// </summary>
    Restore,
    /// <summary>
    /// Sets the show state based on the startup info configuration provided when the process was created.
    /// </summary>
    ShowDefault,
    /// <summary>
    /// Minimizes a window even if the thread that owns the window is not responding.
    /// Should only be used when minimizing windows from a different thread.
    /// </summary>
    ForceMinimize,
}