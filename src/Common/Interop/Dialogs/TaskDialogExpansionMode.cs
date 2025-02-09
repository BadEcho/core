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
/// Specifies where the expanded area of the task dialog is to be displayed.
/// </summary>
public enum TaskDialogExpansionMode
{
    /// <summary>
    /// Expanded area is displayed immediately after the dialog's main content.
    /// </summary>
    ExpandContent,
    /// <summary>
    /// Expanded area is displayed at the bottom of the dialog.
    /// </summary>
    ExpandFooter
}
