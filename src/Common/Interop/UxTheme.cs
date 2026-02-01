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
/// Provides interoperability with Windows theming functionality.
/// </summary>
internal static partial class UxTheme
{
    private const string LIBRARY_NAME = "uxtheme";

    /// <summary>
    /// Influences the effect the user's light/dark mode settings have over context menus.
    /// </summary>
    /// <param name="appMode">An enumeration values that specifies how the user's light/dark mode settings affect context menus.</param>
    /// <returns>
    /// Unclear, given the undocumented nature of this function. Seemingly, zero will be returned if it is successful, I assume a non-zero is returned
    /// if unsuccessful. Big shrug!
    /// </returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "#135", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int SetPreferredAppMode(PreferredAppMode appMode);
}
