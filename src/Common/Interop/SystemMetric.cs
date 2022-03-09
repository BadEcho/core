//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies a system metric or configuration setting that can be requested through <see cref="User32.GetSystemMetrics"/>.
/// </summary>
internal enum SystemMetric
{
    /// <summary>
    /// The width of the screen of the primary display monitor.
    /// </summary>
    PrimaryScreenWidth = 0,
    /// <summary>
    /// The number of display monitors on the desktop.
    /// </summary>
    NumberOfMonitors = 80
}