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

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Specifies a window message sent from a task dialog via a callback function for notification purposes.
/// </summary>
internal enum TaskDialogNotification
{
    /// <summary>
    /// The dialog has been created and not yet displayed.
    /// </summary>
    Created = 0,
    /// <summary>
    /// Navigation has occurred.
    /// </summary>
    Navigated = 1,
    /// <summary>
    /// A button or command link has been clicked.
    /// </summary>
    ButtonClicked = 2,
    /// <summary>
    /// A hyperlink has been clicked.
    /// </summary>
    HyperlinkClicked = 3,
    /// <summary>
    /// The 200 milliseconds period task dialog timer has elapsed, sent if <see cref="TaskDialogFlags.UseCallbackTimer"/>
    /// is specified.
    /// </summary>
    Timer = 4,
    /// <summary>
    /// The window handle is no longer valid.
    /// </summary>
    Destroyed = 5,
    /// <summary>
    /// A radio button has been selected.
    /// </summary>
    RadioButtonClicked = 6,
    /// <summary>
    /// The dialog has been created and not yet displayed.
    /// </summary>
    /// <remarks>
    /// This will be sent before <see cref="Created"/>; for all intents and purposes it is simpler to ignore this notification and
    /// do all initialization logic when handling <see cref="Created"/> instead.
    /// </remarks>
    Constructed = 7,
    /// <summary>
    /// The verification check box has been clicked.
    /// </summary>
    VerificationClicked = 8,
    /// <summary>
    /// The F1 key has been pressed while the dialog had focus.
    /// </summary>
    /// <remarks>
    /// I suppose Microsoft assumes all of its users know that F1 is the universal help key. When handling this notification, it's up to the developer
    /// whether to help the user or laugh at the hapless soul.c
    /// </remarks>
    Help = 9,
    /// <summary>
    /// The "expando" button has been clicked.
    /// </summary>
    ExpandButtonClicked = 10
}
