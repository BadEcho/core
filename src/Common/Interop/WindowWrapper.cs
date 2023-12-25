//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Collections;

namespace BadEcho.Interop;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided window and the messages it receives.
/// </summary>
public abstract class WindowWrapper : IEventSource<WindowHookProc>
{
    private readonly CachedWeakList<WindowHookProc> _hooks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle to the window being wrapped.</param>
    protected WindowWrapper(WindowHandle handle)
    {
        Require.NotNull(handle, nameof(handle));

        Handle = handle;
    }

    /// <summary>
    /// Gets the handle to the wrapped window.
    /// </summary>
    public WindowHandle Handle 
    { get; init; }

    /// <summary>
    /// Gets the callback that processes messages sent to the wrapped window by calling any registered hooks.
    /// </summary>
    protected WindowHookProc WindowProcedure
        => WndProc;

    /// <summary>
    /// Adds a hook that will receive messages prior to any existing hooks receiving them.
    /// </summary>
    /// <param name="hook">The hook to invoke when messages are sent to the wrapped window.</param>
    public void AddStartingHook(WindowHookProc hook)
    {
        _hooks.Insert(0, hook);
    }

    /// <inheritdoc/>
    public void AddHook(WindowHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        _hooks.Add(hook);

        OnHookAdded(hook);
    }

    /// <inheritdoc/>
    public void RemoveHook(WindowHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        _hooks.Remove(hook);

        OnHookRemoved(hook);
    }

    /// <summary>
    /// Called when a hook has been added to the wrapped window.
    /// </summary>
    /// <param name="addedHook">The hook that was added.</param>
    protected virtual void OnHookAdded(WindowHookProc addedHook)
    { }

    /// <summary>
    /// Called when a hook has been removed from the wrapped window.
    /// </summary>
    /// <param name="removedHook">The hook that was removed.</param>
    protected virtual void OnHookRemoved(WindowHookProc removedHook)
    { }

    /// <summary>
    /// Called when the wrapped window is in the process of being destroyed.
    /// </summary>
    /// <remarks>Override this to engage in any last minute cleanup efforts.</remarks>
    protected virtual void OnDestroyingWindow()
    { }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var result = IntPtr.Zero;
        var message = (WindowMessage) msg;

        foreach (WindowHookProc hook in _hooks)
        {
            result = hook(hWnd, msg, wParam, lParam, ref handled);
                    
            if (handled)
                break;
        }

        if (WindowMessage.CreateNonclientArea == message)
        {
            handled = false;
        }
        else if (WindowMessage.DestroyNonclientArea == message)
        {
            OnDestroyingWindow();
            // We want to make sure we always pass on WM_NCDESTROY messages.
            handled = false;
        }

        return result;
    }
}
