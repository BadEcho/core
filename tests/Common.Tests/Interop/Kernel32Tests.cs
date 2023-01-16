//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

/// <summary>
/// These methods are less for testing the unmanaged functions in question (which one should assume Microsoft themselves have tested),
/// and more for testing my P/Invoke signatures.
/// </summary>
public class Kernel32Tests
{
    [Fact]
    public void GetModuleHandle_NoName_ReturnsValid()
    {
        IntPtr hModule = Kernel32.GetModuleHandle(null);

        Assert.NotEqual(IntPtr.Zero, hModule);
    }

    [Fact]
    public void GetModuleHandle_User32_ReturnsValid()
    {
        IntPtr hModule = User32.GetModuleHandle();

        Assert.NotEqual(IntPtr.Zero, hModule);
    }

    [Fact]
    public void GetProcAddress_ExportDefWindowProcW_ReturnsValid()
    {
        IntPtr hModule = User32.GetModuleHandle();
        IntPtr hProc = Kernel32.GetProcAddress(hModule, User32.ExportDefWindowProcW);

        Assert.NotEqual(IntPtr.Zero, hProc);
    }

    [Fact]
    public void WaitForMultipleObjectsEx_Timeout_ReturnsValid()
    {
        int retVal = Kernel32.WaitForMultipleObjectsEx(1, new[] { Process.GetCurrentProcess().Handle }, false, 10, false);

        Assert.Equal(0x102, retVal);
    }
}
