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

using System.ComponentModel;
using System.Runtime.InteropServices;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided out-of-process window and the messages it receives.
/// </summary>
/// <remarks>
/// <para>
/// Window messages intended for other applications cannot be intercepted via the usual technique of subclassing, as it is not
/// possible to subclass a window created by and running on another process.
/// </para>
/// <para>
/// Instead, several global hook procedures are installed and associated with the thread owning the wrapped window. This
/// provides us with all pertinent window message traffic, even across process boundaries. Registering a callback for a
/// global hook procedure requires more than just a function pointer to a managed delegate, however. Global hooks require
/// the injection of a native, standard Windows DLL containing the hook procedure into a target process.
/// </para>
/// <para> 
/// Self-injection of this assembly isn't going to cut it, as a managed DLL simply cannot be loaded in an unmanaged environment,
/// which will fail to load our not-so-standard DLLs due to the lack of a DllMain entry point. 
/// </para>
/// <para>
/// Luckily for us, or maybe just me, we have the native BadEcho.Hooks library, written in C++ and super injectable! If this DLL
/// can be located for the necessary platform invokes, the necessary hooks are established and instances of said native DLL
/// are loaded into the address space of our target processes.
/// </para>
/// <para>
/// In order for our hooking DLL to be able to communicate back to our managed code, we create a message-only window that is
/// set up to receive messages from unmanaged-land. So...there you go. Spy away!
/// </para>
/// </remarks>
public sealed class GlobalWindowWrapper : WindowWrapper, IDisposable
{
    private readonly MessageOnlyExecutor _hookExecutor;
    private readonly int _threadId;
    
    private bool _disposed;
    private bool _windowProcPreviewHooked;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalWindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle to the window being wrapped.</param>
    public GlobalWindowWrapper(WindowHandle handle) 
        : base(handle)
    {
        _threadId = (int) User32.GetWindowThreadProcessId(Handle, IntPtr.Zero);

        if (_threadId == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        _hookExecutor = new MessageOnlyExecutor();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_windowProcPreviewHooked) 
            Hooks.RemoveHook(HookType.WindowProcPreview, _threadId);

        _hookExecutor.Dispose();
        
        _disposed = true;
    }

    /// <inheritdoc/>
    protected override void OnHookAdded(WindowHookProc addedHook)
    {
        base.OnHookAdded(addedHook);

        InitializeGlobalHook();
    }

    private async void InitializeGlobalHook()
    {
        if (_hookExecutor.Window != null)
            return;
         
        await _hookExecutor.RunAsync();
            
        if (_hookExecutor.Window == null)
            throw new InvalidOperationException(Strings.GlobalHookMessageQueueFailed);

        _hookExecutor.Window.AddHook(WindowProcedure);

        _windowProcPreviewHooked
            = Hooks.AddHook(HookType.WindowProcPreview,
                            _threadId,
                            _hookExecutor.Window.Handle);
    }
}
