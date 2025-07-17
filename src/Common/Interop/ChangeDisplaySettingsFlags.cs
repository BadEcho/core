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
/// Specifies flags for changing display settings.
/// </summary>
[Flags]
internal enum ChangeDisplaySettingsFlags
{
    /// <summary>
    /// Causes any staged settings to be applied to the specified device.
    /// </summary>
    Commit = 0x0,
    /// <summary>
    /// The graphics mode for the specified device will be changed dynamically and the graphics mode will be
    /// updated in the registry.
    /// </summary>
    UpdateRegistry = 0x1,
    /// <summary>
    /// The system tests if the requested graphics mode is valid.
    /// </summary>
    Test = 0x2,
    /// <summary>
    /// This is a confusing flag which seems less to do with enabling fullscreen, and more to do with making any
    /// requested changes temporary in nature.
    /// </summary>
    Fullscreen = 0x4,
    /// <summary>
    /// The settings will be saved in the global settings area so that they will affect all users on the machine.
    /// </summary>
    Global = 0x8,
    /// <summary>
    /// The specified device will become the primary device.
    /// </summary>
    SetPrimary = 0x10,
    /// <summary>
    /// When set, the <c>lParam</c> parameter points to a structure containing information for a video connection.
    /// </summary>
    VideoParameters = 0x20,
    /// <summary>
    /// Enables unsafe graphics modes.
    /// </summary>
    EnableUnsafeModes = 0x100,
    /// <summary>
    /// Disables unsafe graphics modes.
    /// </summary>
    DisableUnsafeModes = 0x200,
    /// <summary>
    /// The settings will be saved in the registry, but will not take effect.
    /// </summary>
    NoReset = 0x10000000,
    /// <summary>
    /// I cannot find any documentation out there on this flag. Use <see cref="Reset"/> instead.
    /// </summary>
    ResetEx = 0x20000000,
    /// <summary>
    /// The settings should be changed, even if the requested settings are the same as the current settings.
    /// </summary>
    Reset = 0x40000000
}
