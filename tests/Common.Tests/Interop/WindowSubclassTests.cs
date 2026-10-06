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

using System.ComponentModel;
using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public class WindowSubclassTests
{
    [Fact]
    public void Attach_DestroyedWindow_ThrowsWin32Exception()
    {
        WindowHandle handle = TestWindow.Create("WindowSubclassTests.Attach_DestroyedWindow");
        IntPtr hWnd = handle.DangerousGetHandle();

        handle.Dispose();

        using var subclass = new WindowSubclass(PassThrough);

        Assert.Throws<Win32Exception>(() => subclass.Attach(new WindowHandle(hWnd, false)));
    }

    [Fact]
    public void Attach_AlreadyAttached_ThrowsInvalidOperationException()
    {
        using WindowHandle handle = TestWindow.Create("WindowSubclassTests.Attach_AlreadyAttached");
        using var subclass = new WindowSubclass(PassThrough);

        subclass.Attach(handle);

        Assert.Throws<InvalidOperationException>(() => subclass.Attach(handle));
    }

    [Fact]
    public void Attach_Disposed_ThrowsObjectDisposedException()
    {
        using WindowHandle handle = TestWindow.Create("WindowSubclassTests.Attach_Disposed");
        var subclass = new WindowSubclass(PassThrough);

        subclass.Dispose();

        Assert.Throws<ObjectDisposedException>(() => subclass.Attach(handle));
    }

    private static ProcedureResult PassThrough(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        => new(IntPtr.Zero, false);
}
