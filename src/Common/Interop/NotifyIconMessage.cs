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
/// Specifies a type of message sent to the taskbar's status area.
/// </summary>
public enum NotifyIconMessage
{
    /// <summary>
    /// A message corresponding to NIM_ADD, sent to the taskbar in order to add an icon to the status area.
    /// </summary>
    Add = 0,
    /// <summary>
    /// A message corresponding to NIM_MODIFY, sent to the taskbar in order to modify an icon in the status area.
    /// </summary>
    Modify = 0x1,
    /// <summary>
    /// A message corresponding to NIM_DELETE, sent to the taskbar in order to delete an icon from the status area.
    /// </summary>
    Delete = 0x2,
    /// <summary>
    /// A message corresponding to NIM_SETFOCUS, sent to the taskbar in order to return focus to the taskbar notification
    /// area.
    /// </summary>
    SetFocus = 0x3,
    /// <summary>
    /// A message corresponding to NIM_SETVERSION, sent to the taskbar in order to instruct the notification area to behave
    /// according to the version number specified.
    /// </summary>
    SetVersion = 0x4
}
