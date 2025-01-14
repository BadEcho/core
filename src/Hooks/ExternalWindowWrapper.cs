//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using BadEcho.Hooks.Interop;
using BadEcho.Hooks.Properties;
using BadEcho.Interop;
using BadEcho.Logging;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided out-of-process window and the messages it receives.
/// </summary>
/// <remarks>
/// <para>
/// Window messages intended for other applications cannot be intercepted via the usual technique of subclassing, as it is not
/// possible to subclass a window created by and running on another process.
/// </para>
/// <para>
/// Instead, several hook procedures need to be installed and associated with the thread owning the wrapped window. This
/// provides us with all pertinent window message traffic, even across process boundaries. Registering a callback for a
/// non-local hook procedure requires more than just a function pointer to a managed delegate, however. Hooks for threads outside
/// our active process require the injection of a native Windows DLL containing the hook procedure into the target process.
/// </para>
/// <para> 
/// Self-injection of this assembly isn't going to cut it, as a managed DLL cannot be loaded in an unmanaged environment;
/// native code has no idea how to load and execute the code in our managed assemblies.
/// </para>
/// <para>
/// Luckily for us, or maybe just me, we have the native BadEcho.Hooks.Native library, written in C++ and super injectable! If this DLL
/// can be located for the necessary platform invokes, then instances of our native DLL will be loaded into the address space of the target
/// processes, effectively installing the desired hook procedures.
/// </para>
/// <para>
/// In order for our hooking DLL to be able to communicate back to our managed code, we create a message-only window that is
/// set up to receive messages from unmanaged-land. So...there you go. Spy away!
/// </para>
/// <para>
/// See https://badecho.com/index.php/2024/01/13/external-window-messages/ for more information on the topic.
/// </para>
/// </remarks>
public sealed class ExternalWindowWrapper : WindowWrapper, IDisposable
{
    private readonly MessageOnlyExecutor _hookExecutor = new();
    private readonly int _threadId;

    private bool _disposed;
    private bool _windowHooked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalWindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle to the window being wrapped.</param>
    public ExternalWindowWrapper(WindowHandle handle)
        : base(handle)
    {
        _threadId = Handle.GetThreadId();

        CallbackAdded += async (_, _)
            => await HandleCallbackAdded().ConfigureAwait(false);
    }

    private event EventHandler CallbackAdded;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        CloseHook();

        _hookExecutor.Dispose();

        _disposed = true;
    }

    /// <inheritdoc/>
    protected override void OnCallbackAdded(WindowProcedure addedCallback)
    {
        base.OnCallbackAdded(addedCallback);

        CallbackAdded.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    protected override void OnDestroyingWindow()
    {
        base.OnDestroyingWindow();

        CloseHook();
    }

    private void CloseHook()
    {
        if (!_windowHooked)
            return;

        _windowHooked = !Native.RemoveHook(HookType.CallWindowProcedure, _threadId);

        if (_windowHooked)
            Logger.Warning(Strings.UnhookWindowFailed.InvariantFormat(_threadId));
    }

    private async Task HandleCallbackAdded()
    {
        if (_hookExecutor.Window != null)
            return;

        await _hookExecutor.RunAsync();

        if (_hookExecutor.Window == null)
            throw new InvalidOperationException(Strings.MessageQueueForHookFailed);

        _hookExecutor.Window.AddCallback(WindowProcedure);

        _windowHooked = Native.AddHook(HookType.CallWindowProcedure,
                                       _threadId,
                                       _hookExecutor.Window.Handle);
    }
}