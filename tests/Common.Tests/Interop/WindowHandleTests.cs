// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
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

[Collection("MessageOnlyExecutor")]
public class WindowHandleTests
{
    private static readonly IntPtr _DefaultWindowProc
        = Kernel32.GetProcAddress(User32.GetModuleHandle(), User32.ExportDefWindowProcW);

    // Held statically so the native callback outlives every window whose class uses it.
    private static readonly WNDPROC _DefaultingWindowProc
        = (hWnd, msg, wParam, lParam) => User32.CallWindowProc(_DefaultWindowProc, hWnd, (WindowMessage)msg, wParam, lParam);

    [Fact]
    public async Task Dispose_OtherThread_WindowDestroyedByAnotherThread()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        // The window is created on the executor's thread, which keeps processing its messages.
        WindowHandle? handle
            = executor.Invoke(() => TestWindow.Create(nameof(Dispose_OtherThread_WindowDestroyedByAnotherThread), _DefaultingWindowProc));

        Assert.NotNull(handle);
        Assert.False(handle.IsInvalid);

        IntPtr hWnd = handle.DangerousGetHandle();

        handle.Dispose();

        Assert.True(SpinWait.SpinUntil(() => !WindowExists(hWnd), TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void Dispose_OwnerThread_WindowDestroyed()
    {
        WindowHandle handle = TestWindow.Create(nameof(Dispose_OwnerThread_WindowDestroyed), _DefaultingWindowProc);
        IntPtr hWnd = handle.DangerousGetHandle();

        handle.Dispose();

        Assert.False(WindowExists(hWnd));
    }

    private static bool WindowExists(IntPtr hWnd)
        => User32.GetWindowThreadProcessId(hWnd, out _) != 0;
}
