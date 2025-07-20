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
using Xunit;

namespace BadEcho.Tests.Interop;

public class DisplayTests
{
    [Fact]
    public void EnumDisplayMonitors_AllMonitors_ReturnsTrue()
    {
        var closure = new MonitorCallbackClosure();

        Assert.True(User32.EnumDisplayMonitors(DeviceContextHandle.InvalidHandle, IntPtr.Zero, closure.Callback, IntPtr.Zero));
    }

    [Fact]
    public void MonitorEnumProc_AllMonitors_ReturnsMonitors()
    {
        var monitors = UnmanagedHelper.EnumerateMonitors();

        Assert.NotEmpty(monitors);
    }

    [Fact]
    public void GetMonitorInfo_FirstMonitor_ReturnsTrue()
    {
        var monitors = UnmanagedHelper.EnumerateMonitors();

        MONITORINFOEX monitorInfo = MONITORINFOEX.CreateWritable();

        Assert.True(User32.GetMonitorInfo(monitors.First(), ref monitorInfo));
    }
    
    [Fact]
    public void EnumDisplayDevices_FirstDevice_ReturnsValid()
    {
        var device = new DisplayDevice();

        bool success = User32.EnumDisplayDevices(null, 0, ref device, 0);
        
        Assert.True(success);
    }

    [Fact]
    public void EnumDisplaySettings_CurrentDevice_ReturnsValid()
    {
        var mode = new DeviceMode();

        bool success = User32.EnumDisplaySettings(null, User32.EnumCurrentSettings, ref mode);
        
        Assert.True(success);
    }

    [Fact]
    public void Display_Devices_NotEmpty()
    {
        var displays = Display.Devices;
        
        Assert.NotEmpty(displays);
    }

    [SkipOnGitHubFact]
    public void MakePrimary_LastDisplay_ReturnsValid()
    {
        var displays = Display.Devices;

        var lastDisplay = displays.Skip(2).First();

        lastDisplay.MakePrimary();
    }
}