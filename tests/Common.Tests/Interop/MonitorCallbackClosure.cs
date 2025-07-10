#pragma warning disable IDE0060
// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;

namespace BadEcho.Tests.Interop;

/// <summary>
/// A test referencing environment for <see cref="MonitorEnumProc"/> callbacks.
/// </summary>
public sealed class MonitorCallbackClosure
{
    public ICollection<IntPtr> Monitors
    { get; } = new List<IntPtr>();

    public bool Callback(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr lParam)
    {
        Monitors.Add(hMonitor);

        return true;
    }
}