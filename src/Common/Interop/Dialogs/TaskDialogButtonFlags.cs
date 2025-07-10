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
/// Specifies flags that control which standard buttons appear on a task dialog.
/// </summary>
[Flags]
internal enum TaskDialogButtonFlags
{
    /// <summary>
    /// A standard "OK" button.
    /// </summary>
    OK = 0x1,
    /// <summary>
    /// A standard "Yes" button.
    /// </summary>
    Yes = 0x2,
    /// <summary>
    /// A standard "No" button.
    /// </summary>
    No = 0x4,
    /// <summary>
    /// A standard "Cancel" button.
    /// </summary>
    Cancel = 0x8,
    /// <summary>
    /// A standard "Retry" button.
    /// </summary>
    Retry = 0x10,
    /// <summary>
    /// A standard "Close" button.
    /// </summary>
    Close = 0x20,
    /// <summary>
    /// A standard "Abort" button.
    /// </summary>
    Abort = 0x10000,
    /// <summary>
    /// A standard "Ignore" button.
    /// </summary>
    Ignore = 0x20000,
    /// <summary>
    /// A standard "Try Again" button.
    /// </summary>
    TryAgain = 0x40000,
    /// <summary>
    /// A standard "Continue" button.
    /// </summary>
    Continue = 0x80000,
    /// <summary>
    /// A standard "Help" button.
    /// </summary>
    Help = 0x100000
}
