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

using BadEcho.Collections;

namespace BadEcho.Interop;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided window and the messages it receives.
/// </summary>
public abstract class WindowWrapper : IMessageSource<WindowProcedure>
{
    private readonly DelegateInvocationList<WindowProcedure> _callbacks = [];

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

    /// <inheritdoc/>
    public void AddCallback(WindowProcedure callback)
    {
        Require.NotNull(callback, nameof(callback));

        _callbacks.Add(callback);

        OnCallbackAdded(callback);
    }

    /// <inheritdoc/>
    public void RemoveCallback(WindowProcedure callback)
    {
        Require.NotNull(callback, nameof(callback));

        _callbacks.Remove(callback);

        OnCallbackRemoved(callback);
    }
    
    /// <summary>
    /// Called when a callback function has been added to the wrapped window.
    /// </summary>
    /// <param name="addedCallback">The callback function that was added.</param>
    protected virtual void OnCallbackAdded(WindowProcedure addedCallback)
    { }

    /// <summary>
    /// Called when a callback function has been removed from the wrapped window.
    /// </summary>
    /// <param name="removedCallback">The callback function that was removed.</param>
    protected virtual void OnCallbackRemoved(WindowProcedure removedCallback)
    { }

    /// <summary>
    /// Called when the wrapped window is in the process of being destroyed.
    /// </summary>
    /// <remarks>Override this to engage in any last minute cleanup efforts.</remarks>
    protected virtual void OnDestroyingWindow()
    { }
    
    /// <summary>
    /// The callback, or window procedure, that processes messages sent to the wrapped window by calling any registered callbacks.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>
    /// The result of the message processing, which of course depends on the type of message being processed.
    /// </returns>
    protected ProcedureResult WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        bool forceUnhandled = false;
        var result = new ProcedureResult(IntPtr.Zero, false);
        var message = (WindowMessage) msg;

        foreach (WindowProcedure callback in _callbacks)
        {
            result = callback(hWnd, msg, wParam, lParam);

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
