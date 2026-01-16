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

using BadEcho.Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BadEcho.Hooks.Interop;

/// <summary>
/// Provides interoperability with the BadEcho.Hooks native API.
/// </summary>
internal static partial class Native
{
    private const string LIBRARY_NAME = "BadEcho.Hooks.Native";

    /// <summary>
    /// Installs a new Win32 hook procedure into the specified thread.
    /// </summary>
    /// <param name="hookType">The type of hook procedure to install.</param>
    /// <param name="destination">A handle to the window that will receive messages sent to the hook procedure.</param>
    /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.U1)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    public static partial bool AddHook(HookType hookType, WindowHandle destination, int threadId);

    /// <summary>
    /// Uninstalls a Win32 hook procedure from the specified thread.
    /// </summary>
    /// <param name="hookType">The type of hook procedure to uninstall.</param>
    /// <param name="threadId">The identifier of the thread to remove the hook procedure from.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.U1)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    public static partial bool RemoveHook(HookType hookType, int threadId);

    /// <summary>
    /// Changes the details of a hook message currently being intercepted.
    /// </summary>
    /// <param name="message">The message identifier to use.</param>
    /// <param name="wParam">Additional information about the message to use.</param>
    /// <param name="lParam">Additional information about the message to use.</param>
    [LibraryImport(LIBRARY_NAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    public static partial void ChangeMessageDetails(uint message, IntPtr wParam, IntPtr lParam);
}
