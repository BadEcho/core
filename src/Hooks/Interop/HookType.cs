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

namespace BadEcho.Hooks.Interop;

/// <summary>
/// Specifies a type of hook procedure.
/// </summary>
public enum HookType
{
    /// <summary>
    /// A hook procedure whose type corresponds to <c>WH_CALLWNDPROC</c> that monitors messages before
    /// the system sends them to the destination window procedure.
    /// </summary>
    CallWindowProcedure,
    /// <summary>
    /// A hook procedure whose type corresponds to <c>WH_CALLWNDPROCRET</c> that monitors messages after they
    /// have been processed by the destination window procedure.
    /// </summary>
    CallWindowProcedureReturn,
    /// <summary>
    /// A hook procedure whose type corresponds to <c>WH_GETMESSAGE</c> that monitors messages posted to a message queue prior to
    /// their retrieval.
    /// </summary>
    GetMessage,
    /// <summary>
    /// A hook procedure whose type corresponds to <c>WH_KEYBOARD</c> that monitors keystroke messages.
    /// </summary>
    Keyboard,
    /// <summary>
    /// A hook procedure whose type corresponds to <c>WH_KEYBOARD_LL</c> that monitors low-level keyboard input events.
    /// </summary>
    LowLevelKeyboard
}
