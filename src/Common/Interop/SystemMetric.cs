//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
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