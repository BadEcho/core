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
    /// Provides interoperability with the shell scaling functionality of Windows.
    /// </summary>
    internal static class ShellScaling
    {
        private const string LIBRARY_NAME = "shcore";

        /// <summary>
        /// Queries the dots per inch (DPI) of a display.
        /// </summary>
        /// <param name="hMonitor">A handle of the monitor being queried.</param>
        /// <param name="dpiType">A enumeration value that specifies the type of DPI being queried.</param>
        /// <param name="dpiX">The value of the DPI along the x-axis.</param>
        /// <param name="dpiY">The value of the DPI along the y-axis.</param>
        /// <returns>The result of the operation.</returns>
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern ResultHandle GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
    }
}
