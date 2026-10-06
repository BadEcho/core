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
}