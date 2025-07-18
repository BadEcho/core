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
/// Specifies a system metric or configuration setting that can be requested through <see cref="User32.GetSystemMetrics"/>.
/// </summary>
internal enum SystemMetric
{
    /// <summary>
    /// The width of the screen of the primary display device.
    /// </summary>
    PrimaryScreenWidth = 0,
    /// <summary>
    /// The default width of an icon, in pixels.
    /// </summary>
    IconWidth = 11,
    /// <summary>
    /// The default height of an icon, in pixels.
    /// </summary>
    IconHeight = 12,
    /// <summary>
    /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; zero if menus are left-aligned.
    /// </summary>
    MenuAlignment = 40,
    /// <summary>
    /// The recommended width of a small icon, in pixels.
    /// </summary>
    /// <remarks>
    /// Small icons will typically appear in window captions, small icon views, and the taskbar's notification area.
    /// </remarks>
    SmallIconWidth = 49,
    /// <summary>
    /// The recommended height of a small icon, in pixels.
    /// </summary>
    /// <remarks>
    /// Small icons will typically appear in window captions, small icon views, and the taskbar's notification area.
    /// </remarks>
    SmallIconHeight = 50,
    /// <summary>
    /// The number of display monitors on the desktop.
    /// </summary>
    NumberOfMonitors = 80
}