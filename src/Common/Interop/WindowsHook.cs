//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Specifies a type of hook procedure to install.
/// </summary>
internal enum WindowsHook
{
    /// <summary>
    /// A hook procedure that records input messages posted to the system message queue.
    /// </summary>
    JournalRecord = 0,
    /// <summary>
    /// A hook procedure that receives notifications useful to shell applications.
    /// </summary>
    Shell,
    /// <summary>
    /// A hook procedure that monitors low-level keyboard input events.
    /// </summary>
    KeyboardLowLevel = 13
}
