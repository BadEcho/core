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
/// Specifies a type of hook procedure.
/// </summary>
internal enum HookType
{
    /// <summary>
    /// Monitors <c>WH_CALLWNDPROC</c> messages before the system sends them to a destination window procedure.
    /// </summary>
    CallWindowProcedure,
    /// <summary>
    /// Monitors <c>WH_CALLWNDPROCRET</c> messages after they have been processed by the destination window
    /// procedure.
    /// </summary>
    CallWindowProcedureReturn,
    /// <summary>
    /// Monitors <c>WH_GETMESSAGE</c> messages posted to a message queue prior to their retrieval.
    /// </summary>
    GetMessage
}
