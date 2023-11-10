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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using BadEcho.Logging;
using BadEcho.Properties;
using BadEcho.Threading;

namespace BadEcho.Interop;

/// <summary>
/// Provides a way to subclass a window in a managed environment.
/// </summary>
/// <remarks>
/// <para>
/// This class gives managed objects the ability to subclass an unmanaged window or control. If you are unfamiliar with
/// subclassing, it is a Microsoft term for how one can go about changing or adding additional features to an existing
/// control or window.
/// </para>
/// <para>
/// Put more succinctly, it allows us to replace the window's default window procedure with our own so that we may intercept
/// and process messages sent to the window.
/// </para>
/// </remarks>
internal sealed class WindowSubclass : IDisposable
{
    private static readonly WindowMessage _DetachMessage
        = User32.RegisterWindowMessage("WindowSubclass.DetachMessage");

    private static readonly Dictionary<WindowSubclass, WindowHandle> _SubclassHandleMap
        = new();

    private static readonly object _MapLock
        = new();

    private static readonly IntPtr _DefaultWindowProc 
        = GetDefaultWindowProc();

    private static readonly AssemblyLoadContext _LoadContext;

    [ThreadStatic]
    private static SubclassOperationParameters? _OperationCallbackParameters;

    private static int _ShutdownHandled;

    private readonly ThreadExecutorOperationCallback _operationCallback;
    private readonly IThreadExecutor? _executor;

    private bool _disposed;

    /// <summary>
    /// <para>
    /// A <see cref="GCHandle"/> is employed by this class so it won't get collected even in the event that all managed
    /// references to it get released. This is very important because the oh-so-relevant unmanaged component at hand (i.e., the
    /// window we're subclassing) will still have a reference to us, and this is a situation very much outside the purview of the
    /// .NET garbage collector.
    /// </para>
    /// <para>
    /// Premature cleanup could result in situations where the unmanaged component attempts to call into freshly deallocated
    /// managed memory, which would cause us quite a nasty program crash.
    /// </para>
    /// </summary>
    private GCHandle _gcHandle;
    private WeakReference? _hook;
    private WindowHandle? _window;
    private AttachmentState _state;
    private WindowProc? _wndProcCallback;
    private IntPtr _wndProc;
    private IntPtr _oldWndProc;

    /// <summary>
    /// Initializes the <see cref="WindowSubclass"/> class.
    /// </summary>
    static WindowSubclass()
    {
        // We ensure we're listening to the particular load context responsible for loading Bad Echo framework code;
        // we can't assume we're always a static dependency.
        _LoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())
            ?? AssemblyLoadContext.Default;

        _LoadContext.Unloading += HandleContextUnloading;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSubclass"/> class.
    /// </summary>
    /// <param name="hook">The delegate that will be executed to process messages that are sent or posted to the window.</param>
    /// <param name="executor">The executor that will power the subclass.</param>
    public WindowSubclass(WindowHookProc hook, IThreadExecutor executor)
        : this(hook)
    {
        Require.NotNull(executor, nameof(executor));

        _executor = executor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSubclass"/> class.
    /// </summary>
    /// <param name="hook">The delegate that will be executed to process messages that are sent or posted to the window.</param>
    /// <remarks>
    /// Subclassing without the use of a <see cref="IThreadExecutor"/> will result in direct invocation of the message processing
    /// hook. Use of an executor is recommended if support for more complex functionality such as async operations is desired.
    /// </remarks>
    public WindowSubclass(WindowHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));
        
        _hook = new WeakReference(hook);

        // A reference is required to this method in order to avoid GC collection while the callback is still being referenced by unmanaged
        // objects. Figuring out the root cause of the kind of crashes brought about by this sort of problem is not easy!
        _operationCallback = ExecuteHook;

        _gcHandle = GCHandle.Alloc(this);
    }
    
    /// <summary>
    /// Attaches to and effectively subclasses the provided window by changing the address of its
    /// <see cref="WindowAttribute.WindowProcedure"/>.
    /// </summary>
    /// <param name="window">The window to subclass.</param>
    public void Attach(WindowHandle window)
    {
        Require.NotNull(window, nameof(window));
        
        IntPtr oldWndProc = User32.GetWindowLongPtr(window, WindowAttribute.WindowProcedure);

        Attach(window, WndProc, oldWndProc);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _hook = null;

        Unhook(false);

        _disposed = true;
    }

    /// <summary>
    /// Processes messages sent to the subclassed window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>Value indicating the success of the operation.</returns>
    /// <remarks>
    /// <para>
    /// Attachment and thus the subclassing of the window is performed here if <see cref="Attach(WindowHandle)"/> has not
    /// already been called. This sort of scenario is possible if a window class was manually registered, with its
    /// <see cref="WindowClass.WindowProc"/> set to this method.
    /// </para>
    /// <para>
    /// Otherwise, this method will only begin to process messages once <see cref="Attach(WindowHandle)"/> has been executed.
    /// </para>
    /// </remarks>
    internal IntPtr WndProc(IntPtr hWnd, uint msg, nint wParam, nint lParam)
    {
        var result = IntPtr.Zero;
        var message = (WindowMessage)msg;
        bool handled = false;

        switch (_state)
        {
            case AttachmentState.Unattached:
                var window = new WindowHandle(hWnd, false);
                Attach(window, WndProc, _DefaultWindowProc);
                break;

            case AttachmentState.Detached:
                throw new InvalidOperationException(Strings.SubclassDetachedWndProc);
        }

        IntPtr oldWndProc = _oldWndProc;

        if (_DetachMessage == message)
        {
            if (IntPtr.Zero == wParam || wParam == (IntPtr) _gcHandle)
            {
                bool forcibly = lParam > 0;

                result = Detach(forcibly) ? new IntPtr(1) : IntPtr.Zero;

                handled = !forcibly;
            }
        }
        else
        {
            if (_executor is not { IsShutdownComplete: true })
                result = SendOperation(hWnd, msg, wParam, lParam, ref handled);

            if (WindowMessage.DestroyNonclientArea == message)
            {
                Detach(true);
                // WM_NCDESTROY should always be passed down the chain.
                handled = false;
            }
        }

        // If the message wasn't handled, pass it up the WndProc chain.
        if (!handled)
            result = User32.CallWindowProc(oldWndProc, hWnd, message, wParam, lParam);

        return result;
    }

    private static IntPtr GetDefaultWindowProc()
    {
        IntPtr hModule = User32.GetModuleHandle();

        return Kernel32.GetProcAddress(hModule, User32.ExportDefWindowProcW);
    }

    private static void RestoreDefaultWindowProc(WindowHandle window, IntPtr defaultWindowProc)
    {
        if (window.IsInvalid)
            return;

        if (defaultWindowProc == IntPtr.Zero)
            return;

        IntPtr result = User32.SetWindowLongPtr(window, WindowAttribute.WindowProcedure, defaultWindowProc);

        if (result == IntPtr.Zero)
        {   // The only acceptable outcome here is a window handle that is now invalid due to the window being destroyed already.
            var error = (ErrorCode)Marshal.GetLastWin32Error();

            if (error is not ErrorCode.InvalidWindowHandle and not ErrorCode.Success)
                throw new Win32Exception((int)error);
        }

        if (result != IntPtr.Zero)
            User32.PostMessage(window, WindowMessage.Close, IntPtr.Zero, IntPtr.Zero);
    }

    private static void HandleContextUnloading(AssemblyLoadContext loadContext)
    {
        if (Interlocked.Exchange(ref _ShutdownHandled, 1) != 0)
            return;

        _LoadContext.Unloading -= HandleContextUnloading;

        lock (_MapLock)
        {
            foreach (var window in _SubclassHandleMap.Values)
            {
                // Back when multiple AppDomains existed, there was a chance the AppDomain hosting this code could be unloading
                // in a multiple AppDomain environment where a host window belonging to a separate AppDomain had one or more of its
                // child windows subclassed, which meant we needed to notify the parent window in some fashion by sending a blocking
                // detach message. Don't need to worry about that now! I think...
                RestoreDefaultWindowProc(window, _DefaultWindowProc);
            }
        }
    }

    private void Attach(WindowHandle window, WindowProc newWndProcCallback, IntPtr oldWndProc)
    {
        _window = window;
        _state = AttachmentState.Attached;

        _wndProcCallback = newWndProcCallback;
        _wndProc = Marshal.GetFunctionPointerForDelegate(_wndProcCallback);
        _oldWndProc = oldWndProc;

        User32.SetWindowLongPtr(_window, WindowAttribute.WindowProcedure, _wndProc);
        
        lock (_MapLock)
        {
            _SubclassHandleMap[this] = window;
        }
    }

    /// <remarks>
    /// <para>
    /// The <c>forcibly</c> parameter exists because, due to how subclassing works, it is not always possible to safely remove
    /// a particular window procedure from a <see cref="WindowProc"/> chain. "Safely", used in this context, means in a way that
    /// limits the amount of disruption caused to other subclasses that may have contributed to the WndProc chain.
    /// </para>
    /// <para>
    /// If we are not in a position to unhook from the window's message chain with <c>forcibly</c> set to false, then we essentially
    /// leave everything untouched. If instead we force the detachment, then it is guaranteed that <see cref="WndProc"/> and the hook supplied
    /// at initialization will no longer be executed; unfortunately, this guarantee extends to all subclasses appearing before this one
    /// on the <see cref="WindowProc"/> chain as well (bad).
    /// </para>
    /// </remarks>
    private bool Detach(bool forcibly)
    {
        bool detached;

        if (_state is AttachmentState.Detached or AttachmentState.Unattached)
            detached = true;
        else
        {
            _state = AttachmentState.Detaching;

            detached = Unhook(forcibly);
        }

        if (!detached)
            Logger.Warning(forcibly ? Strings.SubclassForcibleDetachmentFailed : Strings.SubclassDetachmentFailed);

        return detached;
    }

    /// <remarks>
    /// <para>
    /// If the current <see cref="WindowProc"/> assigned to the window subclassed by this type is something other than our own,
    /// then that means the window has been subclassed by some other code and our <see cref="WindowProc"/> is no longer at the head.
    /// This also means that we are unable to unhook our own <see cref="WindowProc"/> from the chain without causing disruption to
    /// the other subclasses.
    /// </para>
    /// <para>
    /// Setting <c>forcibly</c> to false will result in our <see cref="WindowProc"/> only being removed if the current <see cref="WindowProc"/>
    /// points to <see cref="WndProc"/>. If this is the case, or if <c>forcibly</c> is true (and we're going to go ahead with a detachment
    /// whether it's disruptive or not), then we restore the original <see cref="WindowProc"/> function that was stored previously during
    /// this subclass's attachment phase.
    /// </para>
    /// <para>
    /// If the detachment occurs, then we also end up freeing the <see cref="GCHandle"/> that was allocated previously, making this class
    /// eligible for garbage collection.
    /// </para>
    /// </remarks>
    private bool Unhook(bool forcibly)
    {
        if (_state is AttachmentState.Unattached or AttachmentState.Detached || _window == null)
            return true;

        if (!forcibly)
        {
            IntPtr currentWndProc = User32.GetWindowLongPtr(_window, WindowAttribute.WindowProcedure);

            forcibly = currentWndProc == _wndProc;
        }

        if (!forcibly)
            return false;

        _state = AttachmentState.Detaching;

        lock (_MapLock)
        {
            _SubclassHandleMap.Remove(this);
        }

        RestoreDefaultWindowProc(_window, _oldWndProc);

        _state = AttachmentState.Detached;
        _oldWndProc = IntPtr.Zero;
        _wndProcCallback = null;
        _wndProc = IntPtr.Zero;

        _gcHandle.Free();

        return true;
    }

    private IntPtr SendOperation(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var result = IntPtr.Zero;

        // The parameters are cached locally, followed by setting the class member for the parameters to null for purposes of reentrancy.
        _OperationCallbackParameters ??= new SubclassOperationParameters();

        SubclassOperationParameters parameters
            = _OperationCallbackParameters with
              {
                  HWnd = hWnd,
                  Msg = msg,
                  WParam = wParam,
                  LParam = lParam
              };

        _OperationCallbackParameters = null;
        object? operationResult = _executor != null
            ? _executor.Invoke(_operationCallback, parameters)
            : _operationCallback(parameters);

        if (operationResult != null)
        {
            result = parameters.Result;
            handled = parameters.Handled;
        }

        _OperationCallbackParameters = parameters;

        return result;
    }

    private object? ExecuteHook(object? argument)
    {
        if (argument == null)
            return null;

        var parameters = (SubclassOperationParameters)argument;

        if (_state == AttachmentState.Attached)
        {   // Here we finalize the passing of a message received by our subclass to the registered hook.
            bool handled = false;

            if (_hook is { Target: WindowHookProc hook })
            {
                parameters
                    = parameters with
                      {
                          Result = hook(parameters.HWnd, parameters.Msg, parameters.WParam, parameters.LParam, ref handled),
                          Handled = handled
                      };
            }
        }

        return parameters;
    }

    /// <summary>
    /// Specifies the state of our subclass's attachment to a window.
    /// </summary>
    private enum AttachmentState
    {
        /// <summary>
        /// The subclass has not been attached to the window.
        /// </summary>
        Unattached,
        /// <summary>
        /// The subclass has been attached to the window.
        /// </summary>
        Attached,
        /// <summary>
        /// The subclass is currently detaching from the window.
        /// </summary>
        Detaching,
        /// <summary>
        /// The subclass, previously attached to the window, is now unattached.
        /// </summary>
        Detached
    }

    private sealed record SubclassOperationParameters(IntPtr HWnd = default,
                                                      IntPtr WParam = default,
                                                      IntPtr LParam = default,
                                                      uint Msg = default,
                                                      IntPtr Result = default,
                                                      bool Handled = false);
}