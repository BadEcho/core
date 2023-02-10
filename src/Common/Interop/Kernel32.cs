//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the base kernel APIs, such as memory management and input/output operations, exposed by Windows.
/// </summary>
internal static partial class Kernel32
{
    private const string LIBRARY_NAME = "kernel32";

    /// <summary>
    /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
    /// </summary>
    /// <param name="lpModuleName">The name of the loaded module (either a .dll or .exe file).</param>
    /// <returns>If successful, the return value is a handle to the specified module; otherwise, a null pointer.</returns>
    /// <remarks>
    /// <para>
    /// This is used to retrieve the handle to a module that has already been loaded by the calling process. No reference
    /// counts are incremented through the use of this function, which means there is no need to clean up anything
    /// returned by this function.
    /// </para>
    /// <para>
    /// If <c>lpModuleName</c> is null, then this will return a handle to the file used to create the calling process.
    /// </para>
    /// </remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr GetModuleHandle(string? lpModuleName);

    /// <summary>
    /// Retrieves the address of an exported function or variable from the specified dynamic-link library.
    /// </summary>
    /// <param name="hModule">A handle to the loaded library module.</param>
    /// <param name="lpProcName">The function name, variable name, or the function's ordinal value.</param>
    /// <returns>If successful, the return value is the address of the exported function; otherwise, a null pointer.</returns>
    /// <remarks>
    /// Unlike many other unmanaged functions that accept string parameters, this function only comes in an ANSI flavor. The strings
    /// need to marshalled using the UTF-8 marshaller because of this. Note that the new <see cref="LibraryImportAttribute"/> lacks
    /// support for <see cref="DllImportAttribute.ThrowOnUnmappableChar"/>, which this function previously used, but hopefully that's
    /// taken care of by the <see cref="StringMarshalling.Utf8"/> marshaller.
    /// </remarks>
    [LibraryImport(LIBRARY_NAME, StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    public static partial IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    /// <summary>
    /// Waits until one or all of the specified objects are in the signaled state, an I/O completion routine or asynchronous
    /// procedure call is queued to the thread, or the time-out interval elapses.
    /// </summary>
    /// <param name="nCount">
    /// The number of object handles to wait for in the array pointed to by the handles parameter (cannot be zero).
    /// </param>
    /// <param name="lpHandles">An array of object handles.</param>
    /// <param name="bWaitAll">
    /// Value indicating if the function should return when the state of all objects are set to signaled, as opposed to only one of the objects.
    /// </param>
    /// <param name="dwMilliseconds">
    /// The time-out interval, in milliseconds. If a nonzero value is specified, the function waits until the specified objects are signaled 
    /// or the interval elapses. 
    /// </param>
    /// <param name="bAlertable">
    /// Value indicating if the function returns when an I/O completion routine or APC is queued, and then executes the routine or APC.
    /// </param>
    /// <returns>If the function succeeds, the return value indicates the event that caused the function to return.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    public static partial int WaitForMultipleObjectsEx(int nCount,
                                                       IntPtr[] lpHandles,
                                                       [MarshalAs(UnmanagedType.Bool)] bool bWaitAll,
                                                       uint dwMilliseconds,
                                                       [MarshalAs(UnmanagedType.Bool)] bool bAlertable);
}