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
/// Provides interoperability with the Windows Desktop Window Manager API.
/// </summary>
internal static partial class DesktopWindowManager
{
    private const string LIBRARY_NAME = "dwmapi";

    /// <summary>
    /// Retrieves the current value of a specified Desktop Window Manager attribute applied to a window.
    /// </summary>
    /// <param name="hWnd">THe handle to the window form which the attribute value is to be retrieved.</param>
    /// <param name="dwAttribute">A flag describing which value to retrieve.</param>
    /// <param name="pvAttribute">
    /// A pointer to a value which, when this function returns successfully, receives the current value of the attribute.
    /// </param>
    /// <param name="cbAttribute">The size, in bytes, of the attribute value being received.</param>
    /// <returns>The result of the operation.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial ResultHandle DwmGetWindowAttribute(
        WindowHandle hWnd, DwmWindowAttribute dwAttribute, out RECT pvAttribute, int cbAttribute);
}
