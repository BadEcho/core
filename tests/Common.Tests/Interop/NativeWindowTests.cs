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

public class NativeWindowTests
{
    // An unusual combination, to lower the chance that another application already holds it.
    private const ModifierKeys HOT_KEY_MODIFIERS = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift;

    [Fact]
    public void RegisterHotKey_SameIdAfterUnregister_Succeeds()
    {
        using WindowHandle handle = TestWindow.Create("NativeWindowTests.RegisterHotKey_SameIdAfterUnregister");
        using var wrapper = new LocalWindowWrapper(handle);
        var window = new NativeWindow(wrapper);

        window.RegisterHotKey(1, HOT_KEY_MODIFIERS, VirtualKey.F24);
        window.UnregisterHotKey(1);
        window.RegisterHotKey(1, HOT_KEY_MODIFIERS, VirtualKey.F24);
        window.UnregisterHotKey(1);
    }

    [Fact]
    public void RegisterHotKey_RetryAfterFailedRegistration_Succeeds()
    {
        using WindowHandle handle = TestWindow.Create("NativeWindowTests.RegisterHotKey_RetryAfterFailedRegistration");
        using var wrapper = new LocalWindowWrapper(handle);
        var window = new NativeWindow(wrapper);

        window.RegisterHotKey(1, HOT_KEY_MODIFIERS, VirtualKey.F23);
        // The same key combination can only be registered once, so this fails.
        Assert.ThrowsAny<Exception>(() => window.RegisterHotKey(2, HOT_KEY_MODIFIERS, VirtualKey.F23));

        window.UnregisterHotKey(1);
        window.RegisterHotKey(2, HOT_KEY_MODIFIERS, VirtualKey.F23);
        window.UnregisterHotKey(2);
    }

    [Fact]
    public void FromProcessId_CurrentProcess_IncludesCreatedWindow()
    {
        const string className = "NativeWindowTests.FromProcessId_CurrentProcess";
        using WindowHandle handle = TestWindow.Create(className);

        IEnumerable<NativeWindow> windows = NativeWindow.FromProcessId(Environment.ProcessId);

        NativeWindow window
            = Assert.Single(windows, w => w.Handle.DangerousGetHandle() == handle.DangerousGetHandle());

        Assert.Equal(className, window.ClassName);
    }
    [Fact]
    public void MakeOverlay_NotLayered_AppliesTransparentAndLayered()
    {
        (WindowHandle handle, LocalWindowWrapper wrapper, NativeWindow window) = CreateWindow(nameof(MakeOverlay_NotLayered_AppliesTransparentAndLayered));

        try
        {
            window.MakeOverlay();

            var extendedStyle = (ExtendedWindowStyles)User32.GetWindowLongPtr(handle, WindowAttribute.ExtendedStyle);

            Assert.True(extendedStyle.HasFlag(ExtendedWindowStyles.Transparent));
            Assert.True(extendedStyle.HasFlag(ExtendedWindowStyles.Layered));
        }
        finally
        {
            DestroyWindow(handle, wrapper);
        }
    }

    [Fact]
    public void MakeOverlay_AlreadyTransparent_AddsLayered()
    {
        (WindowHandle handle, LocalWindowWrapper wrapper, NativeWindow window) = CreateWindow(nameof(MakeOverlay_AlreadyTransparent_AddsLayered));

        try
        {
            window.ChangeExtendedStyles(ExtendedWindowStyles.Transparent, true);

            window.MakeOverlay();

            var extendedStyle = (ExtendedWindowStyles)User32.GetWindowLongPtr(handle, WindowAttribute.ExtendedStyle);

            Assert.True(extendedStyle.HasFlag(ExtendedWindowStyles.Layered));
        }
        finally
        {
            DestroyWindow(handle, wrapper);
        }
    }

    [Fact]
    public void IsCloaked_CloakedOutsideProperty_ReturnsTrue()
    {
        (WindowHandle handle, LocalWindowWrapper wrapper, NativeWindow window) = CreateWindow(nameof(IsCloaked_CloakedOutsideProperty_ReturnsTrue));

        try
        {
            Assert.False(window.IsCloaked);

            ResultHandle hResult = DesktopWindowManager.DwmSetWindowAttribute(handle, DwmWindowAttribute.Cloak, 1, sizeof(int));

            Assert.False(hResult.Failed());
            Assert.True(window.IsCloaked);
        }
        finally
        {
            DestroyWindow(handle, wrapper);
        }
    }

    [Fact]
    public void IsCloaked_SetThenCleared_ReflectsState()
    {
        (WindowHandle handle, LocalWindowWrapper wrapper, NativeWindow window) = CreateWindow(nameof(IsCloaked_SetThenCleared_ReflectsState));

        try
        {
            window.IsCloaked = true;
            Assert.True(window.IsCloaked);

            window.IsCloaked = false;
            Assert.False(window.IsCloaked);
        }
        finally
        {
            DestroyWindow(handle, wrapper);
        }
    }

    private static (WindowHandle Handle, LocalWindowWrapper Wrapper, NativeWindow Window) CreateWindow(string className)
    {
        WindowHandle handle = TestWindow.Create(className);

        Assert.False(handle.IsInvalid);

        var wrapper = new LocalWindowWrapper(handle);

        return (handle, wrapper, new NativeWindow(wrapper));
    }

    private static void DestroyWindow(WindowHandle handle, LocalWindowWrapper wrapper)
    {
        wrapper.Dispose();
        User32.DestroyWindow(handle.DangerousGetHandle());
    }
}