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

using System.Runtime.InteropServices;

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Provides interoperability with Windows Common Controls.
/// </summary>
internal static partial class ComCtl32
{
    private const string LIBRARY_NAME = "ComCtl32";

    /// <summary>
    /// Creates, displays, and operates a task dialog.
    /// </summary>
    /// <param name="pTaskConfig">
    /// A <see cref="TaskDialogConfiguration"/> instance that contains information used to display the task dialog.
    /// </param>
    /// <param name="pnButton">Address of a variable that receives the ID of the selected button.</param>
    /// <param name="pnRadioButton">Address of a variable that receives the ID of the selected radio button.</param>
    /// <param name="pfVerificationFlagChecked">
    /// Address of a variable that receives the checked state of the verification check box.
    /// </param>
    /// <returns>The result of the operation.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial ResultHandle TaskDialogIndirect(ref TaskDialogConfiguration pTaskConfig,
                                                          out int pnButton,
                                                          out int pnRadioButton,
                                                          [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);
    /// <summary>
    /// Sends the specified message to a task dialog.
    /// </summary>
    /// <param name="hWnd">A handle to the task dialog window whose window procedure will receive the message.</param>
    /// <param name="message">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The result of the message processing, which depends on the message sent.</returns>
    public static IntPtr SendTaskDialogMessage(IntPtr hWnd, TaskDialogMessage message, int wParam, long lParam)
        => User32.SendMessage(hWnd, (WindowMessage) message, wParam, new IntPtr(lParam));

    /// <summary>
    /// Sends the specified message and configuration to a task dialog.
    /// </summary>
    /// <param name="hWnd">A handle to the task dialog window whose window procedure will receive the message.</param>
    /// <param name="message">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="pTaskConfig">The task dialog configuration to send along with the message.</param>
    /// <returns>The result of the message processing, wh ich depends on the message sent.</returns>
    /// <remarks>
    /// This is for <see cref="TaskDialogMessage.NavigatePage"/> messages, which require us to marshal a task dialog configuration
    /// instance.
    /// </remarks>
    [LibraryImport(User32.LibraryName, EntryPoint = "SendMessageW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr SendTaskDialogMessage(IntPtr hWnd,
                                                       TaskDialogMessage message,
                                                       int wParam,
                                                       ref TaskDialogConfiguration pTaskConfig);
}
