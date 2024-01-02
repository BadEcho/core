//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
    
    /// <summary>
    /// The callback, or window procedure, that processes messages sent to the wrapped window by calling any registered hooks.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>
    /// The result of the message processing, which of course depends on the message being processed.
    /// </returns>
    protected HookResult WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        bool forceUnhandled = false;
        var result = new HookResult(IntPtr.Zero, false);
        var message = (WindowMessage) msg;

        foreach (WindowHookProc hook in _hooks)
        {
            result = hook(hWnd, msg, wParam, lParam);
                    
            if (result.Handled)
                break;
        }

        if (WindowMessage.CreateNonclientArea == message)
        {
            forceUnhandled = true;
        }
        else if (WindowMessage.DestroyNonclientArea == message)
        {
            OnDestroyingWindow();
            // We want to make sure we always pass on WM_NCDESTROY messages.
            forceUnhandled = true;
        }

        return result with
               {
                   Handled = !forceUnhandled && result.Handled
               };
    }
}
