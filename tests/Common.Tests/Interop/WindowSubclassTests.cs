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

    /// <suppressions>
    /// ReSharper disable AccessToModifiedClosure
    /// </suppressions>
    [Fact]
    public async Task Dispose_FromOtherThread_RestoresWindowProcedure()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        WindowHandle handle = executor.Window!.Handle;
        int callbackCount = 0;

        IntPtr original = executor.Invoke(() => User32.GetWindowLongPtr(handle, WindowAttribute.WindowProcedure));
        LocalWindowWrapper wrapper = executor.Invoke(() => new LocalWindowWrapper(handle))!;

        wrapper.AddCallback((_, _, _, _) =>
        {
            Interlocked.Increment(ref callbackCount);
            return new ProcedureResult(IntPtr.Zero, false);
        });

        // Disposed from the test thread while the executor's thread pumps the window's messages.
        wrapper.Dispose();

        IntPtr restored = executor.Invoke(() => User32.GetWindowLongPtr(handle, WindowAttribute.WindowProcedure));

        Assert.Equal(original, restored);

        int countAfterDispose = Volatile.Read(ref callbackCount);

        User32.SendMessage(handle, WindowMessage.Null, IntPtr.Zero, IntPtr.Zero);

        Assert.Equal(countAfterDispose, Volatile.Read(ref callbackCount));
    }
    
    private static ProcedureResult PassThrough(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        => new(IntPtr.Zero, false);
}
