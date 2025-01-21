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
/// Specifies the behavior of the task dialog.
/// </summary>
[Flags]
internal enum TaskDialogFlags
{
    /// <summary>
    /// Enables hyperlink processing for the content, expanded information, and footer.
    /// </summary>
    EnableHyperlinks = 0x1,
    /// <summary>
    /// The dialog's main icon is used as the primary icon.
    /// </summary>
    UseMainIcon = 0x2,
    /// <summary>
    /// The dialog's footer icon is used as the primary icon.
    /// </summary>
    UseFooterIcon = 0x4,
    /// <summary>
    /// Enables the use of ALT-F4, Escape, and the title bar's close button to close the dialog even if no
    /// cancel button is provided.
    /// </summary>
    AllowCancel = 0x8,
    /// <summary>
    /// The dialog's custom buttons will be displayed as command links.
    /// </summary>
    UseCommandLinks = 0x10,
    /// <summary>
    /// The dialog's custom buttons will be displayed as command links without icons.
    /// </summary>
    UseCommandLinksWithoutIcon = 0x20,
    /// <summary>
    /// The expanded information is displayed at the bottom of the dialog instead of immediately after the content.
    /// </summary>
    ExpandFooterArea = 0x40,
    /// <summary>
    /// The expanded information is displayed when dialog is initially displayed.
    /// </summary>
    ExpandedByDefault = 0x80,
    /// <summary>
    /// The verification checkbox in the dialog is checked when the dialog is initially displayed.
    /// </summary>
    CheckVerificationFlag = 0x100,
    /// <summary>
    /// A progress bar is displayed.
    /// </summary>
    ShowProgressBar = 0x200,
    /// <summary>
    /// A marquee progress bar is displayed.
    /// </summary>
    ShowMarqueeProgressBar = 0x400,
    /// <summary>
    /// The task dialog's callback is to be called approximately every 200 milliseconds.
    /// </summary>
    UseCallbackTimer = 0x800,
    /// <summary>
    /// The task dialog is positioned relative to the owning window.
    /// </summary>
    PositionRelativeToWindow = 0x1000,
    /// <summary>
    /// The text is displayed reading right to left.
    /// </summary>
    RightToLeftLayout = 0x2000,
    /// <summary>
    /// No default radio button will be selected.
    /// </summary>
    NoDefaultRadioButton = 0x4000,
    /// <summary>
    /// The task dialog can be minimized.
    /// </summary>
    CanBeMinimized = 0x8000,
    /// <summary>
    /// The width of the task dialog is determined by the width of its content area
    /// </summary>
    SizeToContent = 0x1000000
}
