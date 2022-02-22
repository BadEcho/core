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
using Xunit;

namespace BadEcho.Tests.Interop;

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