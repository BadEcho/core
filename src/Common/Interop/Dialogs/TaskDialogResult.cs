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
/// Specifies the result of a user's interaction with a dialog.
/// </summary>
internal enum TaskDialogResult
{
    /// <summary>
    /// No button was clicked.
    /// </summary>
    None,
    /// <summary>
    /// The "OK" button was clicked.
    /// </summary>
    OK,
    /// <summary>
    /// The "Cancel" button was clicked.
    /// </summary>
    Cancel,
    /// <summary>
    /// The "Abort" button was clicked.
    /// </summary>
    Abort,
    /// <summary>
    /// The "Retry" button was clicked.
    /// </summary>
    Retry,
    /// <summary>
    /// The "Ignore" button was clicked.
    /// </summary>
    Ignore,
    /// <summary>
    /// The "Yes" button was clicked.
    /// </summary>
    Yes,
    /// <summary>
    /// The "No" button was clicked.
    /// </summary>
    No,
    /// <summary>
    /// The "Close" button was clicked.
    /// </summary>
    Close,
    /// <summary>
    /// The "Help" button was clicked.
    /// </summary>
    Help,
    /// <summary>
    /// The "Try Again" button was clicked.
    /// </summary>
    TryAgain,
    /// <summary>
    /// The "Continue" button was clicked.
    /// </summary>
    Continue
}
