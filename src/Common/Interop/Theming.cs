// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Logging;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Interacts with various theming aspects of Windows.
/// </summary>
public static class Theming
{   // Windows 10 1809, the first build exporting the undocumented menu theming functions.
    private const int DARK_MENUS_MINIMUM_BUILD = 17763;

    /// <summary>
    /// Enables dark mode for context menus if configured by the user.
    /// </summary>
    public static void EnableDarkMenus()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, DARK_MENUS_MINIMUM_BUILD))
        {
            Logger.Warning(Strings.ThemingDarkMenusUnsupported);
            return;
        }

        UxTheme.SetPreferredAppMode(PreferredAppMode.AllowDark);
    }
}
