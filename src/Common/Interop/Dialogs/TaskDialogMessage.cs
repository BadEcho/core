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
/// Specifies a window message sent to a task dialog to affect its appearance or behavior.
/// </summary>
internal enum TaskDialogMessage
{
    /// <summary>
    /// Recreates a task dialog with new contents, simulating the functionality of a multi-page wizard.
    /// </summary>
    NavigatePage = WindowMessage.User + 101,
    /// <summary>
    /// Simulates the action of a button click.
    /// </summary>
    ClickButton = WindowMessage.User + 102,
    /// <summary>
    /// Display a hosted progress bar in marquee mode.
    /// </summary>
    SetMarqueeProgressBar = WindowMessage.User + 103,
    /// <summary>
    /// Sets the state of the progress bar.
    /// </summary>
    SetProgressBarState = WindowMessage.User + 104,
    /// <summary>
    /// Sets the minimum and maximum values for the progress bar.
    /// </summary>
    SetProgressBarRange = WindowMessage.User + 105,
    /// <summary>
    /// Sets the position of the progress bar.
    /// </summary>
    SetProgressBarPosition = WindowMessage.User + 106,
    /// <summary>
    /// Starts and stops the marquee display of the progress bar, and sets the speed of the marquee.
    /// </summary>
    SetProgressBarMarquee = WindowMessage.User + 107,
    /// <summary>
    /// Updates a text element in a task dialog.
    /// </summary>
    SetElementText = WindowMessage.User + 108,
    /// <summary>
    /// Simulates the action of a radio button.
    /// </summary>
    ClickRadioButton = WindowMessage.User + 110,
    /// <summary>
    /// Enables or disables a push button.
    /// </summary>
    EnableButton = WindowMessage.User + 111,
    /// <summary>
    /// Enables or disables a radio button.
    /// </summary>
    EnableRadioButton = WindowMessage.User + 112,
    /// <summary>
    /// Simulates a click of the verification check box, if it exists.
    /// </summary>
    ClickVerification = WindowMessage.User + 113,
    /// <summary>
    /// Updates a text element.
    /// </summary>
    UpdateElementText = WindowMessage.User + 114,
    /// <summary>
    /// Specifies whether a given button or command link should have a UAC shield icon; or, in other words, whether the
    /// action invoked by the button requires elevation.
    /// </summary>
    SetButtonElevationRequiredState = WindowMessage.User + 115,
    /// <summary>
    /// Refreshes the icon of a task dialog.
    /// </summary>
    UpdateIcon = WindowMessage.User + 116
}
