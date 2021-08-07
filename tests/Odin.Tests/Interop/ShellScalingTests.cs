//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using BadEcho.Odin.Interop;
using Xunit;

namespace BadEcho.Odin.Tests.Interop
{
    public class ShellScalingTests
    {
        [Fact]
        public void GetDpiForMonitor_DefaultDpiType_IsValid()
        {
            var monitors = UnmanagedHelper.EnumerateMonitors();

            Assert.Equal(ResultHandle.Success,
                         ShellScaling.GetDpiForMonitor(monitors.First(), MonitorDpiType.Effective, out _, out _));
        }
    }
}
