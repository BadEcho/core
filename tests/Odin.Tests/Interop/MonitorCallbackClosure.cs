#pragma warning disable IDE0060
//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using BadEcho.Odin.Interop;

namespace BadEcho.Odin.Tests.Interop
{
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
}
