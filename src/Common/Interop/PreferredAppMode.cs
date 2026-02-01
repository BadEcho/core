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
/// Specifies light/dark mode settings for context menus.
/// </summary>
internal enum PreferredAppMode
{
    /// <summary>
    /// Default light/dark mode settings for context menus, which, as far as I'm aware, will always result in light mode
    /// menus, regardless of user settings.
    /// </summary>
    Default,
    /// <summary>
    /// Allows context menus to use dark mode if the user has dark mode enabled.
    /// </summary>
    AllowDark,
    /// <summary>
    /// Forces context menus to use dark mode regardless of user settings.
    /// </summary>
    ForceDark,
    /// <summary>
    /// Forces context menus to use light mode regardless of user settings.
    /// </summary>
    ForceLight,
    /// <summary>
    /// No idea what this does. These APIs are undocumented.
    /// </summary>
    Max
}
