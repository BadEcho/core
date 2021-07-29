//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using BadEcho.Odin.Interop;
using Xunit;

namespace BadEcho.Odin.Tests.Interop
{
    public class User32Tests
    {
        [Fact]
        public void EnumDisplayMonitors_ReturnsTrue()
        {
            var closure = new TestCallbackClosure();

            Assert.True(User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, closure.Callback, IntPtr.Zero));
        }

        [Fact]
        public void EnumDisplayMonitors_ReturnsMonitors()
        {
            var closure = new TestCallbackClosure();

            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, closure.Callback, IntPtr.Zero);

            Assert.NotEmpty(closure.Monitors);
        }

        [Fact]
        public void GetMonitorInfo_ReturnsTrue()
        {
            var closure = new TestCallbackClosure();
            
            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, closure.Callback, IntPtr.Zero);

            var monitorInfo = MONITORINFOEX.CreateWritable();
            
            Assert.True(User32.GetMonitorInfo(closure.Monitors.First(), ref monitorInfo));
        }

        private sealed class TestCallbackClosure
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
}
