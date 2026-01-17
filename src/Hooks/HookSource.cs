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

using BadEcho.Extensions;
using BadEcho.Hooks.Interop;
using BadEcho.Hooks.Properties;
using BadEcho.Interop;
using BadEcho.Logging;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of messages from hook events.
/// </summary>
public abstract class HookSource : IDisposable
{
    private readonly MessageOnlyExecutor _hookExecutor = new();
    private readonly HookType _hookType;
    private readonly int _threadId;

    private bool _hooked;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="HookSource"/> class.
    /// </summary>
    /// <param name="hookType">An enumeration value specifying the type of hook procedure to install.</param>
    /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated.</param>
    protected HookSource(HookType hookType, int threadId)
        : this(hookType)
    {
        _threadId = threadId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookSource"/> class.
    /// </summary>
    /// <param name="hookType">An enumeration value specifying the type of hook procedure to install.</param>
    /// <remarks>
    /// Initializing a hook source without specifying a thread ID will result in the hook procedure being installed as a
    /// global hook.
    /// </remarks>
    protected HookSource(HookType hookType)
    {
        _hookType = hookType;
    }

    /// <summary>
    /// Initializes the message loop that facilitates the receiving of hook messages, and then installs the hook procedure.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
    public async Task StartAsync()
    {
        if (_hookExecutor.Window == null)
        {
            await _hookExecutor.StartAsync().ConfigureAwait(false);

            if (_hookExecutor.Window == null)
                throw new InvalidOperationException(Strings.MessagingForHookFailed);

            _hookExecutor.Window.AddCallback(HandleHookEvent);
        }

        if (_hooked)
            return;

        // Some hook procedures, when installed at a global scope, are called in the context of the thread that installed them.
        // This sort of voodoo requires said thread to be pumping a message loop. So, to cover all possible cases, we make sure
        // to install the hook procedure using the local message-only window thread.
        await _hookExecutor.InvokeAsync(() =>
        {
            _hooked = Native.AddHook(_hookType,
                                     _hookExecutor.Window.Handle, 
                                     _threadId);
        });
    }

    /// <summary>
    /// Uninstalls the hook procedure, preventing any more messages from being received.
    /// </summary>
    /// <remarks>
    /// This method is synchronous since uninstalling a hook procedure is a non-blocking operation. The hook procedure
    /// can be reinstalled by calling <see cref="StartAsync"/>, as the message loop running in the background isn't
    /// shut down until <see cref="Dispose()"/> is called.
    /// </remarks>
    public void Stop()
    {
        if (!_hooked)
            return;

        _hooked = !Native.RemoveHook(_hookType, _threadId);

        if (_hooked)
            Logger.Warning(Strings.UnhookFailed.InvariantFormat(_threadId));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and (optionally) managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only managed resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Stop();
            _hookExecutor.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Called by the installed native hook procedure via <c>SendMessage</c>/<c>PostMessage</c> when a hook event has occurred.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    protected abstract void OnHookEvent(nint hWnd, uint msg, nint wParam, nint lParam);

    private ProcedureResult HandleHookEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {   // Ignore all system messages; we're only interested in messages sent by our hook DLL.
        if (msg < (int) WindowMessage.User)
            return new ProcedureResult(IntPtr.Zero, true);

        msg -= (int) WindowMessage.User;

        OnHookEvent(hWnd, msg, wParam, lParam);

        // We always mark our hook messages as handled; we don't want further processing by any supporting
        // infrastructure. This has no bearing on whether or not the next hook procedure in the current hook
        // chain is called, which our hook DLL will always do.
        return new ProcedureResult(IntPtr.Zero, true);
    }
}
