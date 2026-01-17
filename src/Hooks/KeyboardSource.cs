// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Hooks.Interop;
using BadEcho.Hooks.Properties;
using BadEcho.Interop;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of keyboard input.
/// </summary>
public sealed class KeyboardSource : HookSource
{
    private readonly KeyboardProcedure _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardSource"/> class.
    /// </summary>
    /// <param name="callback">The delegate that will be executed when a hook event has occured.</param>
    /// <param name="threadId">The identifier of the thread whose message queue will be monitored for keyboard input.</param>
    public KeyboardSource(KeyboardProcedure callback, int threadId)
        : base(HookType.Keyboard, threadId)
    {
        Require.NotNull(callback, nameof(callback));

        _callback = callback;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardSource"/> class.
    /// </summary>
    /// <param name="callback">The delegate that will be executed when a hook event has occured.</param>
    /// <remarks>This will install a global keyboard hook, capturing keyboard input across all processes.</remarks>
    public KeyboardSource(KeyboardProcedure callback)
        : base(HookType.LowLevelKeyboard)
    {
        Require.NotNull(callback, nameof(callback));

        _callback = callback;
    }

    /// <inheritdoc/>
    protected override void OnHookEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        VirtualKey key = (VirtualKey) wParam;
        KeyState state = (WindowMessage) msg switch
        {
            WindowMessage.KeyDown or WindowMessage.SystemKeyDown => KeyState.Down,
            WindowMessage.KeyUp or WindowMessage.SystemKeyUp => KeyState.Up,
            _ => throw new ArgumentException(Strings.NonKeyboardMessageReceived)
        };

        _callback(state, key);
    }
}
