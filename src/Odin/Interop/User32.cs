//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides interoperability with the core user interface functionality of Windows.
    /// </summary>
    internal static class User32
    {
        private const string LIBRARY_NAME = "user32";

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpRect">
        /// A <see cref="RECT"/> structure that receives the screen coordinates of the upper-left and lower-right corners of
        /// the window.
        /// </param>
        /// <returns>True if the function succeeds; otherwise, false.</returns>
        [DllImport(LIBRARY_NAME, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Retrieves information about a specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="attribute">An enumeration value that specifies the window attribute to retrieve.</param>
        /// <returns>The requested value if successful; otherwise, zero.</returns>
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, WindowAttribute attribute)
            => IntPtr.Size == 4 ? GetWindowLongPtr32(hWnd, (int) attribute) : GetWindowLongPtr64(hWnd, (int) attribute);

        /// <summary>
        /// Changes an attribute for the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="attribute">An enumeration value that specifies the window attribute to replace.</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns></returns>
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowAttribute attribute, IntPtr dwNewLong)
            => IntPtr.Size == 4
                ? SetWindowLongPtr32(hWnd, (int) attribute, dwNewLong)
                : SetWindowLongPtr64(hWnd, (int) attribute, dwNewLong);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be retrieved.</param>
        /// <returns>The requested value if successful; otherwise, zero.</returns>
        /// <remarks>This should only ever be called from a 32-bit machine.</remarks>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be retrieved.</param>
        /// <returns>The requested value if successful; otherwise, zero.</returns>
        /// <remarks>This should only ever be called from a 64-bit machine.</remarks>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Changes an attribute of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be set.</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>The previous value of the specified offset if successful; otherwise, zero.</returns>
        /// <remarks>This should only ever be called from a 32-bit machine.</remarks>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Changes an attribute of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be set.</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>The previous value of the specified offset if successful; otherwise, zero.</returns>
        /// <remarks>This should only ever be called from a 64-bit machine.</remarks>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }
}
