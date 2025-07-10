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
/// Represents a callback invoked by the <see cref="User32.EnumDisplayMonitors"/> function.
/// </summary>
/// <param name="hMonitor">A handle to the display monitor.</param>
/// <param name="hdcMonitor">A handle to the device context.</param>
/// <param name="lprcMonitor">A pointer to the rectangle structure.</param>
/// <param name="lParam">Application-defined data.</param>
/// <returns>Value indicating whether enumeration should continue.</returns>
/// <remarks>
/// If <c>hdcMonitor</c> is not null, then <c>lprcMonitor</c> is the intersection of the clipping area of the device
/// context identified by <c>hdcMonitor</c> and the display monitor rectangle. Otherwise, <c>lprcMonitor</c> is the
/// display monitor rectangle.
/// </remarks>
internal delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr lParam);