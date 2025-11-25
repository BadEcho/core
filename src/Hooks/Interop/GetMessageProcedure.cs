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

using BadEcho.Interop;

namespace BadEcho.Hooks.Interop;

/// <summary>
/// Represents a callback that processes messages about to be returned from a message queue.
/// </summary>
/// <param name="hWnd">A handle to the window.</param>
/// <param name="msg">The message.</param>
/// <param name="wParam">Additional message-specific information.</param>
/// <param name="lParam">Additional message-specific information.</param>
/// <returns>
/// The result of the message processing, which of course depends on the message being processed.
/// </returns>
/// <remarks>
/// Like the <c>GetMsgProc</c> callback function this is based on, this callback allows you to modify the message
/// before the requesting application receives it.
/// </remarks>
public delegate ProcedureResult GetMessageProcedure(nint hWnd, ref uint msg, ref nint wParam, ref nint lParam);
