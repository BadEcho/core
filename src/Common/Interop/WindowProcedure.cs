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
/// Represents a callback that processes messages sent to a window.
/// </summary>
/// <param name="hWnd">A handle to the window.</param>
/// <param name="msg">The message.</param>
/// <param name="wParam">Additional message-specific information.</param>
/// <param name="lParam">Additional message-specific information.</param>
/// <returns>
/// The result of the message processing, which of course depends on the message being processed.
/// </returns>
/// <remarks>
/// This variant of the window procedure delegate is meant to be invoked from managed code, it cannot be invoked
/// directly from unmanaged code. Instead, register a <see cref="WNDPROC"/> delegate as the native callback, which can
/// then invoke managed callbacks that use this delegate.
/// </remarks>
public delegate ProcedureResult WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);