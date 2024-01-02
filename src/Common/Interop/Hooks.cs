//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the Bad Echo global hooks API.
/// </summary>
internal static partial class Hooks
{
    private const string LIBRARY_NAME = "BadEcho.Hooks";
    
    /// <summary>
    /// Installs a new globally-scoped hook procedure into the specified thread.
    /// </summary>
    /// <param name="hookType">The type of hook procedure to install.</param>
    /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated.</param>
    /// <param name="destination">A handle to the window that will receive messages sent to the hook procedure.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    public static partial bool AddHook(HookType hookType, int threadId, WindowHandle destination);

    /// <summary>
    /// Uninstalls a globally-scoped hook procedure from the specified thread.
    /// </summary>
    /// <param name="hookType">The type of hook procedure to uninstall.</param>
    /// <param name="threadId">The identifier of the thread to remove the hook procedure from.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    public static partial bool RemoveHook(HookType hookType, int threadId);
}
