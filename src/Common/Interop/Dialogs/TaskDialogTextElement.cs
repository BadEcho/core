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
/// Specifies a type of task dialog text element.
/// </summary>
internal enum TaskDialogTextElement
{
    /// <summary>
    /// The main content of the dialog.
    /// </summary>
    Content,
    /// <summary>
    /// The expanded information of the dialog.
    /// </summary>
    ExpandedInformation,
    /// <summary>
    /// The footer area of the dialog.
    /// </summary>
    Footer,
    /// <summary>
    /// The main instruction of the dialog.
    /// </summary>
    MainInstruction
}
