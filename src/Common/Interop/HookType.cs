//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
    /// A hook procedure (WH_CALLWNDPROC) that monitors messages before the system sends them to a destination window procedure.
    /// </summary>
    WindowProcPreview,
    /// <summary>
    /// A hook procedure (WH_CALLWNDPROCRET) that monitors messages after they have been processed by the destination window
    /// procedure.
    /// </summary>
    WindowProcReturn,
    /// <summary>
    /// A hook procedure (WH_GETMESSAGE) that monitors messages posted to a message queue.
    /// </summary>
    MessageQueueRead
}
