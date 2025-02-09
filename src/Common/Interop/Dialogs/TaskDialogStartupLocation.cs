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

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Specifies the initial display location for a task dialog.
/// </summary>
public enum TaskDialogStartupLocation
{
    /// <summary>
    /// The dialog will be placed in the center of the screen.
    /// </summary>
    CenterScreen,
    /// <summary>
    /// The dialog will be centered relative to the window that owns the dialog.
    /// </summary>
    CenterOwner
}
