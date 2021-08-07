//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using BadEcho.Odin.Interop;
using Xunit;

namespace BadEcho.Odin.Tests.Interop
{
    /// <summary>
    /// These methods are less for testing the unmanaged functions in question (which one should assume Microsoft themselves have tested),
    /// and more for testing my P/Invoke signatures.
    /// </summary>
    public class User32Tests
    {
        [Fact]
        public void EnumDisplayMonitors_ReturnsTrue()
        {
            var closure = new MonitorCallbackClosure();

            Assert.True(User32.EnumDisplayMonitors(DeviceContextHandle.Null, IntPtr.Zero, closure.Callback, IntPtr.Zero));
        }

        [Fact]
        public void EnumDisplayMonitors_ReturnsMonitors()
        {
            var monitors = UnmanagedHelper.EnumerateMonitors();

            Assert.NotEmpty(monitors);
        }

        [Fact]
        public void GetMonitorInfo_ReturnsTrue()
        {
            var monitors = UnmanagedHelper.EnumerateMonitors();

            var monitorInfo = MONITORINFOEX.CreateWritable();
            
            Assert.True(User32.GetMonitorInfo(monitors.First(), ref monitorInfo));
        }
    }
}
