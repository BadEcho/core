//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
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
/// This class gives managed objects the ability to subclass an unmanaged window or control. If you are unfamiliar with
/// subclassing, it is a Microsoft term for how one can go about changing or adding additional features to an existing
/// control or window.
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
    private static SubclassOperationParameters? _ExecutorOperationCallbackParameters;

    private static int _ShutdownHandled;

    private readonly ThreadExecutorOperationCallback _executorOperationCallback;
    private readonly IThreadExecutor _executor;

    /// <summary>
    /// Initializes the <see cref="WindowSubclass"/> class.
    /// </summary>
    static WindowSubclass()
    {
        // We ensure we're listening to the particular load context responsible for loading Fenestra code; we can't assume we're always a static
        // dependency.
        _LoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())
            ?? AssemblyLoadContext.Default;

        _LoadContext.Unloading += HandleContextUnloading;
    }

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
    /// Initializes a new instance of the <see cref="WindowSubclass"/> class.
    /// </summary>
    /// <param name="hook">The delegate that will be executed to process messages that are sent or posted to the window.</param>
    /// <param name="executor">The executor that will power the subclass.</param>
    public WindowSubclass(WindowProc hook, IThreadExecutor executor)
    {
        Require.NotNull(hook, nameof(hook));
        Require.NotNull(executor, nameof(executor));

        _executor = executor;
        _hook = new WeakReference(hook);

        // A reference is required to this method in order to avoid GC collection while the callback is still being referenced by unmanaged
        // objects. Figuring out the root cause of the kind of crashes brought about by this sort of problem is not easy!
        _executorOperationCallback = ExecuteHook;

        _gcHandle = GCHandle.Alloc(this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _hook = null;

        Unhook(false);
    }

    /// <summary>
    /// Processes messages sent to the subclassed window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>Value indicating the success of the operation.</returns>
    internal IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var result = IntPtr.Zero;
        var message = (WindowMessage)msg;

        switch (_state)
        {
            case AttachmentState.Unattached:
                var window = new WindowHandle(hWnd, false);
                Attach(window, WndProc, Marshal.GetFunctionPointerForDelegate(_DefaultWindowProc));
                break;

            case AttachmentState.Detached:
                throw new InvalidOperationException(Strings.SubclassDetachedWndProc);
        }

        IntPtr oldWndProc = _oldWndProc;

        if (_DetachMessage == message)
            result = ProcessDetachMessage(wParam, lParam);
        else
        {
            if (!_executor.IsShutdownComplete)
                result = SendOperation(hWnd, msg, wParam, lParam);

            if (WindowMessage.DestroyNonclientArea == message)
            {
                Detach(true);
                // WM_NCDESTROY should always be passed down the chain.
                result = new IntPtr(-1);
            }
        }

        // If the message wasn't handled, pass it up the WndProc chain.
        if (IntPtr.Zero != result)
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
    /// a particular window procedure from a <see cref="WindowProc"/> chain. "safely", used in this context, means in a way that
    /// limits the amount of disruption caused to other subclasses that may have contributed to the WndProc chain.
    /// </para>
    /// <para>
    /// If we are not in a position to unhook from the window's message chain, with <c>forcibly</c> set to false, then we essentially
    /// leave everything untouched. If we are forcing a detachment, then it is guaranteed that <see cref="WndProc"/> and the hook supplied
    /// at initialization will no longer be executed, however it can also be guaranteed that other entities subclassing this window will
    /// experience disruption (bad).
    /// </para>
    /// </remarks>
    private void Detach(bool forcibly)
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

    private IntPtr ProcessDetachMessage(IntPtr wParam, IntPtr lParam)
    {
        if (IntPtr.Zero != wParam && wParam != (IntPtr)_gcHandle)
            return IntPtr.Zero;

        int param = (int) lParam;
        bool forcibly = param > 0;

        return Unhook(forcibly) ? IntPtr.Zero : new IntPtr(-1);
    }

    private IntPtr SendOperation(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var result = IntPtr.Zero;

        // The parameters are cached locally, followed by setting the class member for the parameters to null for purposes of reentrancy.
        _ExecutorOperationCallbackParameters ??= new SubclassOperationParameters();

        SubclassOperationParameters parameters = _ExecutorOperationCallbackParameters;

        _ExecutorOperationCallbackParameters = null;

        parameters.HWnd = hWnd;
        parameters.Msg = msg;
        parameters.WParam = wParam;
        parameters.LParam = lParam;
        
        object? executorResult = _executor.Invoke(_executorOperationCallback, true, parameters);

        if (executorResult != null)
            result = parameters.Result;

        _ExecutorOperationCallbackParameters = parameters;

        return result;
    }

    private object? ExecuteHook(object? argument)
    {
        if (argument == null)
            return null;

        var parameters = (SubclassOperationParameters)argument;

        parameters.Result = IntPtr.Zero;

        if (_state == AttachmentState.Attached)
        {   // Here we finalize the passing of a message received by our subclass to the registered hook.
            if (_hook is { Target: WindowProc hook })
                parameters.Result = hook(parameters.HWnd, parameters.Msg, parameters.WParam, parameters.LParam);
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

    /// <summary>
    /// Provides parameters for a subclass executor operation.
    /// </summary>
    private sealed class SubclassOperationParameters
    {
        /// <summary>
        /// Gets or sets a handle to the window.
        /// </summary>
        public IntPtr HWnd
        { get; set; }

        /// <summary>
        /// Gets or sets additional message information.
        /// </summary>
        public IntPtr WParam
        { get; set; }

        /// <summary>
        /// Gets or sets additional message information.
        /// </summary>
        public IntPtr LParam 
        { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public uint Msg
        { get; set; }

        /// <summary>
        /// Gets or sets the result of the message processing.
        /// </summary>
        public IntPtr Result
        { get; set; }
    }
}