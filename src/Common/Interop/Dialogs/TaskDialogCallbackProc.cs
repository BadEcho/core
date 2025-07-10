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

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// An application-defined task dialog callback function.
/// </summary>
/// <param name="hWnd">Handle to the task dialog window.</param>
/// <param name="msg">The notification message being sent.</param>
/// <param name="wParam">Additional notification information.</param>
/// <param name="lParam">Additional notification information.</param>
/// <param name="lpRefData">The value originally passed to the callback data field.</param>
/// <returns>A value specific to the notification being processed.</returns>
internal delegate int TaskDialogCallbackProc(IntPtr hWnd,
                                             TaskDialogNotification msg,
                                             IntPtr wParam,
                                             IntPtr lParam,
                                             IntPtr lpRefData);