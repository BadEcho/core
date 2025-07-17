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
/// Specifies the result of a display settings change.
/// </summary>
internal enum ChangeDisplaySettingsResult
{
    /// <summary>
    /// The settings change was successful.
    /// </summary>
    Successful = 0,
    /// <summary>
    /// The computer must be restarted for the graphics mode to work.
    /// </summary>
    Restart = 1,
    /// <summary>
    /// The display driver failed the specified graphics mode.
    /// </summary>
    Failed = -1,
    /// <summary>
    /// The graphics mode is not supported.
    /// </summary>
    BadMode = -2,
    /// <summary>
    /// Unable to write settings to the registry.
    /// </summary>
    NotUpdated = -3,
    /// <summary>
    /// An invalid set of flags was passed in.
    /// </summary>
    BadFlags = -4,
    /// <summary>
    /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
    /// </summary>
    BadParameter = -5,
    /// <summary>
    /// The settings change was unsuccessful because the system is DualView capable.
    /// </summary>
    BadDualView = -6
}
