// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
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
/// Provides a publisher of mouse input.
/// </summary>
public sealed class MouseSource : HookSource
{
    private readonly MouseProcedure _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseSource"/> class.
    /// </summary>
    /// <param name="callback">The delegate that will be executed when a hook event has occurred.</param>
    /// <param name="threadId">The identifier of the thread whose message queue will be monitored for mouse input.</param>
    public MouseSource(MouseProcedure callback, int threadId)
        : base(HookType.Mouse, threadId)
    {
        Require.NotNull(callback, nameof(callback));

        _callback = callback;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseSource"/> class.
    /// </summary>
    /// <param name="callback">The delegate that will be executed when a hook event has occurred.</param>
    /// <remarks>This will install a global mouse hook, capturing mouse input across all processes.</remarks>
    public MouseSource(MouseProcedure callback)
        : base(HookType.LowLevelMouse)
    {
        Require.NotNull(callback, nameof(callback));

        _callback = callback;
    }

    /// <inheritdoc/>
    protected override void OnHookEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        int x = wParam.ToInt32();
        int y = lParam.ToInt32();

        MouseEvent mouseEvent = (WindowMessage) msg switch
        {
            WindowMessage.LeftButtonDown => MouseEvent.LeftButtonDown,
            WindowMessage.LeftButtonUp => MouseEvent.LeftButtonUp,
            WindowMessage.RightButtonDown => MouseEvent.RightButtonDown,
            WindowMessage.RightButtonUp => MouseEvent.RightButtonUp,
            WindowMessage.MiddleButtonDown => MouseEvent.MiddleButtonDown,
            WindowMessage.MiddleButtonUp => MouseEvent.MiddleButtonUp,
            WindowMessage.MouseMove => MouseEvent.Move,
            WindowMessage.MouseWheel => MouseEvent.Wheel,
            WindowMessage.XButtonDown => MouseEvent.XButtonDown,
            WindowMessage.XButtonUp => MouseEvent.XButtonUp,
            _ => throw new ArgumentException(Strings.NonMouseMessageReceived)
        };

        _callback(mouseEvent, x, y);
    }
}
