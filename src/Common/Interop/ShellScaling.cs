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
/// Provides interoperability with the shell scaling functionality of Windows.
/// </summary>
internal static partial class ShellScaling
{
    private const string LIBRARY_NAME = "shcore";

    /// <summary>
    /// Queries the dots per inch (DPI) of a display.
    /// </summary>
    /// <param name="hMonitor">A handle of the monitor being queried.</param>
    /// <param name="dpiType">An enumeration value that specifies the type of DPI being queried.</param>
    /// <param name="dpiX">The value of the DPI along the x-axis.</param>
    /// <param name="dpiY">The value of the DPI along the y-axis.</param>
    /// <returns>The result of the operation.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError =true)]
    public static partial ResultHandle GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);
}