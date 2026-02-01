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

namespace BadEcho.Interop;

/// <summary>
/// Interacts with various theming aspects of Windows.
/// </summary>
public static class Theming
{
    /// <summary>
    /// Enables dark mode for context menus if configured by the user.
    /// </summary>
    public static void EnableDarkMenus() 
        => UxTheme.SetPreferredAppMode(PreferredAppMode.AllowDark);
}
