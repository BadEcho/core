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
/// Specifies a system command executed from a window menu.
/// </summary>
internal enum SystemCommand
{
    /// <summary>
    /// No command.
    /// </summary>
    None = 0,
    /// <summary>
    /// Retrieves the window's context menu as a result of a keystroke.
    /// </summary>
    KeyMenu = 0xF100,
    /// <summary>
    /// Retrieves the window's context menu as a result of a mouse click.
    /// </summary>
    MouseMenu = 0xF090
}
