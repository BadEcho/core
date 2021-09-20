//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides interoperability with the base APIs, such as memory management and input/output operations, exposed by Windows.
    /// </summary>
    internal static class Kernel32
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
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Retrieves the address of an exported function or variable from the specified dynamic-link library.
        /// </summary>
        /// <param name="hModule">A handle to the loaded library module.</param>
        /// <param name="lpProcName">The function name, variable name, or the function's ordinal value.</param>
        /// <returns>If successful, the return value is the address of the exported function; otherwise, a null pointer.</returns>
        /// <remarks>
        /// Unlike many other unmanaged functions that accept string parameters, this function only comes in an ANSI flavor, so we
        /// tell the runtime to always use ANSI, while also telling the runtime to not look for a non-existent <c>GetProcAddressA</c>.
        /// </remarks>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Ansi, ExactSpelling = true, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string lpProcName);
    }
}
