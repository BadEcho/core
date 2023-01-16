//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.InteropServices;
using BadEcho.Collections;
using BadEcho.Threading;

namespace BadEcho.Interop;

/// <summary>
/// Provides a disposable wrapper around an <c>HWND</c> of a message-only window and the messages it receives.
/// </summary>
public sealed class MessageOnlyWindowWrapper : IWindowWrapper, IDisposable
{
    private readonly CachedWeakList _hooks = new();
    private readonly int _ownerThreadId = Environment.CurrentManagedThreadId;

    private readonly IThreadExecutor _executor;

    private bool _windowIsBeingDestroyed; 
    private ushort _classAtom;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageOnlyWindowWrapper"/> class.
    /// </summary>
    /// <param name="executor">The executor used by the wrapper to execute actions.</param>
    public MessageOnlyWindowWrapper(IThreadExecutor executor)
    {
        Require.NotNull(executor, nameof(executor));

        _executor = executor;

        var subclass = new WindowSubclass(WndProc, executor);

        // We store a reference to the initial WndProc so that it isn't garbage collected before our subclass replaces the WndProc with its
        // own following the first message it receives after our call to CreateWindowEx.
        WindowProc initialCallback = subclass.WndProc;
        string className = CreateClassName();

        _classAtom = RegisterClass(initialCallback, className);
        Handle = WindowHandle.InvalidHandle;

        try
        {
            unsafe
            {
                Handle = User32.CreateWindowEx(0,
                                               className,
                                               string.Empty,
                                               0,
                                               0,
                                               0,
                                               0,
                                               0,
                                               User32.ParentWindowMessageOnly,
                                               IntPtr.Zero,
                                               IntPtr.Zero,
                                               null);
            }
        }
        finally
        {
            if (Handle.IsInvalid)
            {   // The subclass pins itself, so if window creation fails, we need to manually release it here and now.
                subclass.Dispose();
            }
        }

        // This is required to guarantee that the initial WindowProc delegate callback is kept alive throughout the method.
        GC.KeepAlive(initialCallback);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MessageOnlyWindowWrapper"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Yes people, this is one of those extremely rare cases where a finalizer override is required. It is required because
    /// one of the unmanaged resources that this class creates and holds a reference to cannot be maintained by a <see cref="SafeHandle"/>
    /// instance.
    /// </para>
    /// <para>
    /// This class is responsible for creating not only a window, but also a window class. Both of these resources must be cleaned up
    /// when this class is disposed. The class especially must be cleaned up, as it will persist even beyond the unloading of the DLL
    /// responsible for using this type (although, thankfully, only up until the process exits, hopefully). This leads to all sorts of
    /// bad things that can happen.
    /// </para>
    /// <para>
    /// We cannot create a <see cref="SafeHandle"/> for the window class, as the native function that creates the window class returns
    /// a value of type <c>ATOM</c>, as opposed to a handle. Proper usage of safe handles dictates that they be created through the
    /// P/Invoke system, not by user code, and thus must be in the P/Invoke signature as its return type. Since the marshaller will only
    /// implicitly wrap <see cref="IntPtr"/>-valued handles with safe handles, we're precluded from creating a safe handle type for
    /// a window class.
    /// </para>
    /// <para>
    /// Therefore, because we cannot create a safe handle for this unique unmanaged type of resource, we must ensure its cleanup by
    /// overriding this class's finalizer method. 
    /// </para>
    /// </remarks>
    ~MessageOnlyWindowWrapper()
    {
        Dispose();
    }

    /// <summary>
    /// Gets the handle to the wrapped window.
    /// </summary>
    public WindowHandle Handle
    { get; }

    /// <summary>
    /// Adds a hook that will receive messages prior to any existing hooks receiving them.
    /// </summary>
    /// <param name="hook">The hook to invoke when messages are sent to the wrapped window.</param>
    public void AddStartingHook(WindowProc hook)
    {
        _hooks.Insert(0, hook);
    }

    /// <inheritdoc/>
    public void AddHook(WindowProc hook)
    {
        _hooks.Add(hook);
    }

    /// <inheritdoc/>
    public void RemoveHook(WindowProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        _hooks.Remove(hook);
    }

    /// <inheritdoc/>
    public void Dispose()
    {   // Since WM_NCDESTROY messages are propagated, there is a chance this method will be invoked multiple times.
        if (_disposed)
            return;

        _disposed = true;

        if (_windowIsBeingDestroyed)
        {   // Since the window is in the process of being destroyed, we can't call UnregisterClass yet. So, we basically
            // post it to the executor for it to happen later, once the window is closed.
            _executor.BeginInvoke(() => UnregisterClass(_classAtom), true, null);
        }
        else if (!Handle.IsInvalid)
        {   // Actions such as destroying the window and unregistering its class should only be done on the window's own thread.
            if (Environment.CurrentManagedThreadId == _ownerThreadId)
                DestroyWindow(Handle, _classAtom);
            else
                _executor.BeginInvoke(() => DestroyWindow(Handle, _classAtom), true, null);
        }

        _classAtom = 0;

        GC.SuppressFinalize(this);
    }

    private static string CreateClassName()
    {   // The class name needs to be unique so we can be confident that class registration won't fail.
        // To achieve this, we base the name off the application's name concatenated with the thread name,
        // and a random GUID. We just need to be mindful of the 255 character limit for class names.
        string applicationName = AppDomain.CurrentDomain.FriendlyName.Length >= 128
            ? AppDomain.CurrentDomain.FriendlyName[..128]
            : AppDomain.CurrentDomain.FriendlyName;

        string threadName = Thread.CurrentThread.Name?.Length >= 64
            ? Thread.CurrentThread.Name[..64]
            : Thread.CurrentThread.Name ?? string.Empty;

        return $"{applicationName}.{threadName}.{Guid.NewGuid()}";
    }

    private static ushort RegisterClass(WindowProc initialCallback, string className)
    {
        IntPtr hNullBrush = Gdi32.GetStockObject(Gdi32.StockObjectBrushNull);

        if (hNullBrush == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        IntPtr hInstance = Kernel32.GetModuleHandle(null);

        var windowClass = new WindowClass(initialCallback)
                          {
                              BackgroundBrush = hNullBrush,
                              ClassExtraBytes = 0,
                              ClassName = className,
                              Cursor = IntPtr.Zero,
                              Icon = IntPtr.Zero,
                              Instance = hInstance,
                              MenuName = string.Empty,
                              SmallIcon = IntPtr.Zero,
                              Style = 0,
                              WindowExtraBytes = 0
                          };
        
        return User32.RegisterClassEx(ref windowClass);
    }

    private static void DestroyWindow(WindowHandle handle, ushort classAtom)
    {
        handle.Close();

        UnregisterClass(classAtom);
    }

    private static void UnregisterClass(ushort classAtom)
    {
        if (classAtom == 0)
            return;

        IntPtr hInstance = User32.GetModuleHandle();

        if (User32.UnregisterClass(new IntPtr(classAtom), hInstance) == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var result = IntPtr.Zero;
        var message = (WindowMessage) msg;

        foreach (WindowProc hook in _hooks)
        {   // A value of zero indicates that the hook has handled the particular message being passed.
            result = hook(hWnd, msg, wParam, lParam);
                    
            if (result == IntPtr.Zero)
                break;
        }

        if (WindowMessage.CreateNonclientArea == message)
        {
            result = new IntPtr(1);
        }
        else if (WindowMessage.DestroyNonclientArea == message)
        {   // Time to cleanup, though the class unregistration needs to be delayed since the window is already being destroyed.
            _windowIsBeingDestroyed = true;
            Dispose();

            // We want to make sure we always pass on WM_NCDESTROY messages.
            result = new IntPtr(-1);
        }

        return result;
    }
}
