#pragma warning disable IDE0060
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