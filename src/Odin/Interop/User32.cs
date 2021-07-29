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
        /// Enumerates display monitors (including invisible pseudo-monitors associated with the mirroring drivers) that intersect
        /// a region formed by the intersection of a specified clipping rectangle and the visible region of a device context.
        /// </summary>
        /// <param name="hdc">Handle to a display device context that defines the visible region of interest.</param>
        /// <param name="lprcClip">Pointer to a <see cref="RECT"/> structure that specifies a clipping rectangle.</param>
        /// <param name="lpfnEnum">Callback invoked by this method with monitor information.</param>
        /// <param name="lParam">Application-defined data that is passed to the provided callback.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr lParam);

        /// <summary>
        /// Retrieves information about a display monitor.
        /// </summary>
        /// <param name="hMonitor">Handle to the display monitor of interest.</param>
        /// <param name="lpmi">
        /// Pointer to a properly initialized <see cref="MONITORINFOEX"/> structure, which is written to by this function.
        /// </param>
        /// <returns>True if successful; otherwise, false.</returns>
        [DllImport(LIBRARY_NAME, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] ref MONITORINFOEX lpmi);

        /// <summary>
        /// Retrieves the specified system metric or configuration setting.
        /// </summary>
        /// <param name="nIndex">The system metric or configuration setting to retrieve.</param>
        /// <returns>If successful, the request system metric or configuration setting; otherwise, zero.</returns>
        [DllImport(LIBRARY_NAME, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int GetSystemMetrics(SystemMetric nIndex);

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
        public static extern bool GetWindowRect(IntPtr hWnd, [Out] out RECT lpRect);

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
