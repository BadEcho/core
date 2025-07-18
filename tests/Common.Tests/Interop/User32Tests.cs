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

using System.Runtime.InteropServices;
using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

/// <summary>
/// These methods are less for testing the unmanaged functions in question (which one should assume Microsoft themselves have tested),
/// and more for testing my P/Invoke signatures.
/// </summary>
public class User32Tests
{
    [Fact]
    public void RegisterClassEx_ReturnsValid()
    {
        ushort atom = TestWindow.RegisterClass("RegisterClassEx_ReturnsValid");  

        Assert.NotEqual(0, atom);
    }

    [Fact]
    public void UnregisterClass_ReturnsValid()
    {
        ushort atom = TestWindow.RegisterClass("UnregisterClass_ReturnsValid");

        IntPtr hInstance = Kernel32.GetModuleHandle(null);
        IntPtr pAtom = new (atom);

        Assert.NotEqual(0, User32.UnregisterClass(pAtom, hInstance));
    }

    [Fact]
    public void CreateWindowEx_ReturnsValid()
    {
        WindowHandle handle = TestWindow.Create("CreateWindowEx_ReturnsValid");

        Assert.False(handle.IsInvalid);
    }

    [Fact]
    public void DestroyWindow_ReturnsValid()
    {
        WindowHandle handle = TestWindow.Create("DestroyWindow_ReturnsValid");

        IntPtr pHandle = handle.DangerousGetHandle();

        Assert.True(User32.DestroyWindow(pHandle));
    }

    [Fact]
    public void GetWindowRect_ReturnsValid()
    {
        WindowHandle handle = TestWindow.Create("GetWindowRect_ReturnsValid");

        Assert.True(User32.GetWindowRect(handle, out _));
    }

    [Fact]
    public void GetSystemMetric_PrimaryScreenWidth_ReturnsValid()
    {
        Assert.NotEqual(0, User32.GetSystemMetrics(SystemMetric.PrimaryScreenWidth));
    }

    [Fact]
    public void GetDpiForSystem_ReturnsValid()
    {
        Assert.True(User32.GetDpiForSystem() > 0);
    }

    [Fact]
    public void GetDC_ReturnsValid()
    {
        DeviceContextHandle handle = User32.GetDC(WindowHandle.InvalidHandle);

        Assert.False(handle.IsInvalid);
    }

    [Fact]
    public void ReleaseDC_ReturnsValid()
    {
        DeviceContextHandle handle = User32.GetDC(WindowHandle.InvalidHandle);

        Assert.Equal(1, User32.ReleaseDC(IntPtr.Zero, handle.DangerousGetHandle()));
    }

    [Fact]
    public void GetMessage_PostMessage_ReturnsTrue()
    {
        var msg = new MSG();

        var getMessage = Task.Run(() =>
                                  {
                                      WindowHandle handle = TestWindow.Create("GetMessage_ReturnsTrue");

                                      Assert.True(User32.GetMessage(ref msg, handle.DangerousGetHandle(), 0, 0));
                                  });

        User32.SendMessage(WindowHandle.InvalidHandle, (WindowMessage) 1, IntPtr.Zero, IntPtr.Zero);

        getMessage.Wait();
    }

    [Fact]
    public void TranslateMessage_NoMessage_ReturnsFalse()
    {
        var msg = new MSG();

        Assert.False(User32.TranslateMessage(ref msg));
    }

    [Fact]
    public void DispatchMessage_NoMessage_ReturnsZero()
    {
        var msg = new MSG();

        Assert.Equal(IntPtr.Zero, User32.DispatchMessage(ref msg));
    }

    [Fact]
    public void CallWindowProc_ReturnsValid()
    {
        WindowHandle handle = TestWindow.Create("CallWindowProc_ReturnsValid");
        WNDPROC wndProc = TestWindow.WndProc;

        Assert.NotEqual(IntPtr.Zero,
                        User32.CallWindowProc(Marshal.GetFunctionPointerForDelegate(wndProc),
                                              handle.DangerousGetHandle(),
                                              WindowMessage.Null,
                                              IntPtr.Zero,
                                              IntPtr.Zero));
    }

    [Fact]
    public void RegisterHotKey_CtrlZ_ReturnsTrue()
    {
        WindowHandle handle = TestWindow.Create("RegisterHotKey_CtrlA_ReturnsTrue");

        Assert.True(User32.RegisterHotKey(handle, 1, ModifierKeys.Control, VirtualKey.Z));
    }

    [Fact]
    public void UnregisterHotKey_NothingRegistered_ReturnsFalse()
    {
        WindowHandle handle = TestWindow.Create("UnregisterHotKey_NothingRegistered_ReturnsFalse");

        Assert.False(User32.UnregisterHotKey(handle, 1));
    }

    [Fact]
    public void RegisterWindowMessage_Hello_ReturnsValid()
    {
        WindowMessage message = User32.RegisterWindowMessage("Hello");

        Assert.NotEqual(0, (int) message);
    }

    [Fact]
    public void GetThreadDpiAwarenessContext_Default_ReturnsValid()
    {
        var context = User32.GetThreadDpiAwarenessContext();
        Assert.Equal(DpiAwareness.Unaware, User32.GetAwarenessFromDpiAwarenessContext(context));
    }
}