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

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the Windows Shell API.
/// </summary>
internal static partial class Shell32
{
    private const string LIBRARY_NAME = "shell32";

    /// <summary>
    /// Sends a message to the taskbar's status area.
    /// </summary>
    /// <param name="dwMessage">A value that specifies the action to be taken by this function.</param>
    /// <param name="lpData">
    /// A <see cref="NotifyIconData"/> instance containing notification area information.
    /// </param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "Shell_NotifyIconW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool Shell_NotifyIcon(NotifyIconMessage dwMessage, ref NotifyIconData lpData);

    /// <summary>
    /// Executes a specified operation on a file, folder, or application, such as opening, printing, or editing, by
    /// invoking the Windows Shell.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the parent window used for displaying any message boxes or user interface prompts.
    /// Can be zero if no window handle is available.
    /// </param>
    /// <param name="lpOperation">The action to be performed.</param>
    /// <param name="lpFile">The file or object on which to execute the specified operation. </param>
    /// <param name="lpParameters">The parameters to pass to the application if the file is an executable.</param>
    /// <param name="lpDirectory">
    /// The working directory for the operation. If this parameter is null, the current directory is used.
    /// </param>
    /// <param name="nShowCmd">
    /// The flags that specify how the application window is to be shown. This value corresponds to the <c>nCmdShow</c> parameter
    /// used by the <see cref="User32.ShowWindow"/> function.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is greater than 32; otherwise, an error value is returned that indicates
    /// the cause of the failure.
    /// </returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "ShellExecuteW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ShellExecute(nint hWnd,
                                           string lpOperation,
                                           string? lpFile,
                                           string? lpParameters,
                                           string? lpDirectory,
                                           int nShowCmd);
}
