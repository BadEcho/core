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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the core user interface functionality of Windows.
/// </summary>
internal static partial class User32
{
    private const string LIBRARY_NAME = "user32";

    /// <summary>
    /// Gets the name for the exported <c>DefWindowProcW</c> function.
    /// </summary>
    public static string ExportDefWindowProcW
        => "DefWindowProcW";

    /// <summary>
    /// The value to pass as the handle to the parent window when creating a message-only window.
    /// </summary>
    public static IntPtr ParentWindowMessageOnly
        => new(-3);

    /// <summary>
    /// A hook code indicating that the hook procedure was provided with valid data and may process it.
    /// </summary>
    public static int HookCodeAction
        => 0;

    /// <summary>
    /// Creates an overlapped, pop-up, or child window with an extended window style.
    /// </summary>
    /// <param name="dwExStyle">The extended window style of the window being created.</param>
    /// <param name="lpClassName">String or class atom created by a previous call to the <see cref="RegisterClassEx"/> function.</param>
    /// <param name="lpWindowName">The window name.</param>
    /// <param name="style">The style of the window being created.</param>
    /// <param name="x">The initial horizontal position of the window.</param>
    /// <param name="y">The initial vertical position of the window.</param>
    /// <param name="width">The width, in device units, of the window.</param>
    /// <param name="height">The height, in device units, of the window.</param>
    /// <param name="hWndParent">A handle to the parent or owner window.</param>
    /// <param name="hMenu">A handle to a menu.</param>
    /// <param name="hInstance">A handle to the instance of the module to be associated with the window.</param>
    /// <param name="lpParam">Pointer to a value to be passed to the window.</param>
    /// <returns>A handle to the new window if successful; otherwise, null.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "CreateWindowExW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static unsafe partial WindowHandle CreateWindowEx(int dwExStyle,
                                                             string lpClassName,
                                                             string lpWindowName,
                                                             int style,
                                                             int x,
                                                             int y,
                                                             int width,
                                                             int height,
                                                             IntPtr hWndParent,
                                                             IntPtr hMenu,
                                                             IntPtr hInstance,
                                                             void* lpParam);
    /// <summary>
    /// Destroys the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window to be destroyed.</param>
    /// <returns>If success, true; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool DestroyWindow(IntPtr hWnd);

    /// <summary>
    /// Registers a window class for subsequent use.
    /// </summary>
    /// <param name="wc_d">A <see cref="WindowClass"/> instance containing window class information.</param>
    /// <returns>A class atom that uniquely identifies the class; otherwise, zero.</returns>
    /// <suppressions>
    /// ReSharper disable InconsistentNaming
    /// </suppressions>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "RegisterClassExW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial ushort RegisterClassEx([MarshalUsing(typeof(WindowClassMarshaller))] ref WindowClass wc_d);

    /// <summary>
    /// Unregisters a window class, freeing the memory required for the class.
    /// </summary>
    /// <param name="classAtom">The class atom.</param>
    /// <param name="hInstance">A handle to the instance of the module that created the class.</param>
    /// <returns>If successful, a nonzero value; otherwise, zero.</returns>
    /// <remarks>Before unregistering a window class, ensure all windows created with said class have been destroyed.</remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "UnregisterClassW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int UnregisterClass(IntPtr classAtom, IntPtr hInstance);

    /// <summary>
    /// Enumerates display monitors (including invisible pseudo-monitors associated with the mirroring drivers) that intersect
    /// a region formed by the intersection of a specified clipping rectangle and the visible region of a device context.
    /// </summary>
    /// <param name="hdc">A handle to a display device context that defines the visible region of interest.</param>
    /// <param name="lprcClip">Pointer to a <see cref="RECT"/> structure that specifies a clipping rectangle.</param>
    /// <param name="lpfnEnum">Callback invoked by this method with monitor information.</param>
    /// <param name="lParam">Application-defined data that is passed to the provided callback.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool EnumDisplayMonitors(DeviceContextHandle hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr lParam);

    /// <summary>
    /// Retrieves information about a display monitor.
    /// </summary>
    /// <param name="hMonitor">A handle to the display monitor of interest.</param>
    /// <param name="lpmi">
    /// A <see cref="MONITORINFOEX"/> value, which is written to by this function.
    /// </param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "GetMonitorInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    /// <summary>
    /// Retrieves the specified system metric or configuration setting.
    /// </summary>
    /// <param name="nIndex">An enumeration value that specifies the system metric or configuration setting to retrieve.</param>
    /// <returns>If successful, the request system metric or configuration setting; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int GetSystemMetrics(SystemMetric nIndex);

    /// <summary>
    /// Retrieves the dimensions of the bounding rectangle of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpRect">
    /// A <see cref="RECT"/> structure that receives the screen coordinates of the upper-left and lower-right corners of
    /// the window.
    /// </param>
    /// <returns>True if the function succeeds; otherwise, false.</returns>
    //[DllImport(LIBRARY_NAME, ExactSpelling = true, SetLastError = true)]
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool GetWindowRect(WindowHandle hWnd, out RECT lpRect);

    /// <summary>
    /// Retrieves a handle to a device context (DC) for the client area of either a specified window or for the entire screen.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window whose DC is to be retrieved. A value of <see cref="IntPtr.Zero"/> will retrieve the DC for the entire
    /// screen.
    /// </param>
    /// <returns>If successful, a handle to the DC; otherwise, null.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial DeviceContextHandle GetDC(IntPtr hWnd);

    /// <summary>
    /// Releases a device context (DC), freeing it for use by other applications.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hdc">A handle to the DC to be released.</param>
    /// <returns>A return value of one if successful; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ReleaseDC(IntPtr hWnd, IntPtr hdc);

    /// <summary>
    /// Retrieves a message from the calling thread's message queue.
    /// </summary>
    /// <param name="lpMsg">
    /// A pointer to a <see cref="MSG"/> structure that receives message information from the thread's message queue.
    /// </param>
    /// <param name="hWnd">
    /// A handle to the window whose messages are to be retrieved. If null (zero), messages for any window belonging to the current
    /// thread are retrieved.
    /// </param>
    /// <param name="uMsgFilterMin">The integer value of the lowest message value to be retrieved.</param>
    /// <param name="uMsgFilterMax">The integer value of the highest message value to be retrieved.</param>
    /// <returns>If successful, true; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "GetMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool GetMessage(ref MSG lpMsg, IntPtr hWnd, uint uMsgFilterMin, uint uMsgFilterMax);

    /// <summary>
    /// Sends the specified message to one or more window.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose window procedure will receive the message. If</param>
    /// <param name="msg">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The result of the message processing, which depends on the message sent.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "SendMessageW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr SendMessage(WindowHandle hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Translates virtual-key messages into character messages.
    /// </summary>
    /// <param name="lpMsg">
    /// A pointer to a <see cref="MSG"/> structure that contains message information retrieved from the calling thread's message queue.
    /// </param>
    /// <returns>True if the message is translated; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool TranslateMessage(ref MSG lpMsg);

    /// <summary>
    /// Dispatches a message to a window procedure.
    /// </summary>
    /// <param name="lpMsg">A pointer to a <see cref="MSG"/> structure that contains the message.</param>
    /// <returns>Value returned by the window procedure.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "DispatchMessageW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr DispatchMessage(ref MSG lpMsg);

    /// <summary>
    /// Retrieves a module handle to the module that this class provides interoperability with.
    /// </summary>
    /// <returns>A handle to the user32.dll module.</returns>
    public static IntPtr GetModuleHandle()
        => Kernel32.GetModuleHandle($"{LIBRARY_NAME}.dll");

    /// <summary>
    /// Retrieves information about a specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="attribute">An enumeration value that specifies the window attribute to retrieve.</param>
    /// <returns>The requested value if successful; otherwise, zero.</returns>
    public static IntPtr GetWindowLongPtr(WindowHandle hWnd, WindowAttribute attribute)
        => IntPtr.Size == 4 ? GetWindowLongPtr32(hWnd, (int) attribute) : GetWindowLongPtr64(hWnd, (int) attribute);

    /// <summary>
    /// Changes an attribute for the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="attribute">An enumeration value that specifies the window attribute to replace.</param>
    /// <param name="dwNewLong">The replacement value.</param>
    /// <returns>The previous value of the specified offset if successful; otherwise, zero.</returns>
    public static IntPtr SetWindowLongPtr(WindowHandle hWnd, WindowAttribute attribute, IntPtr dwNewLong)
        => IntPtr.Size == 4
            ? SetWindowLongPtr32(hWnd, (int) attribute, dwNewLong)
            : SetWindowLongPtr64(hWnd, (int) attribute, dwNewLong);

    /// <summary>
    /// Defines a system-wide hot key.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window that will receive <see cref="WindowMessage.HotKey"/> messages generated by the hot key.
    /// </param>
    /// <param name="id">The identifier of the hot key.</param>
    /// <param name="fsModifiers">The keys that must be pressed in combination with the specified virtual key.</param>
    /// <param name="vk">The virtual-key code of the hot key.</param>
    /// <returns>If successful, true; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool RegisterHotKey(WindowHandle hWnd, int id, ModifierKeys fsModifiers, VirtualKey vk);

    /// <summary>
    /// Frees a hot key previously registered by the calling thread.
    /// </summary>
    /// <param name="hWnd">A handle to the window associated with the hot key to be freed.</param>
    /// <param name="id">The identifier of the hot key to be freed.</param>
    /// <returns>If successful, true; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool UnregisterHotKey(WindowHandle hWnd, int id);

    /// <summary>
    /// Retrieves the status of the specified virtual key.
    /// </summary>
    /// <param name="key">The virtual key to get the state for.</param>
    /// <returns>
    /// A value specifying the status of <c>key</c> as follows:
    ///     <list type="bullet">
    ///         <item>If the high-order bit is 1, <c>key</c> is down; otherwise, it is up.</item>
    ///         <item>If the low-order bit is 1, <c>key</c> is toggled on; otherwise, it is toggled off.</item>
    ///     </list>
    /// </returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial short GetKeyState(VirtualKey key);

    /// <summary>
    /// Defines a new window message that is guaranteed to be unique throughout the system.
    /// </summary>
    /// <param name="msg">The message to be registered.</param>
    /// <returns>The message identifier if successful; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "RegisterWindowMessageW", StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial WindowMessage RegisterWindowMessage(string msg);

    /// <summary>
    /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns
    /// without waiting for the thread to process the message.
    /// </summary>
    /// <param name="hWnd">
    /// Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, 
    /// the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, 
    /// overlapped windows, and pop-up windows; but, the message is not sent to child windows.
    /// </param>
    /// <param name="msg">Specifies the message to be sent.</param>
    /// <param name="wParam">Specifies additional message-specific information.</param>
    /// <param name="lParam">Specifies additional message-specific information.</param>
    /// <returns>Value indicating the success of the operation.</returns>
    /// <returns>If successful, true; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool PostMessage(WindowHandle hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Passes message information to the specified window procedure.
    /// </summary>
    /// <param name="wndProc">The previous window procedure.</param>
    /// <param name="hWnd">A handle to the window procedure to receive the message.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The result of the message processing.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "CallWindowProcW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Installs an application-defined hook procedure into a hook chain, used to monitor the system for certain
    /// types of events.
    /// </summary>
    /// <param name="hookId">The type of hook procedure to be installed.</param>
    /// <param name="lpfn">The hook procedure to install in the chain.</param>
    /// <param name="hMod">A handle to the DLL containing the hook procedure.</param>
    /// <param name="dwThreadId">
    /// The identifier of the thread with which the hook procedure is to be associated. For desktop apps, this should be zero.
    /// </param>
    /// <returns>The handle to the hook procedure if successful; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr SetWindowsHookEx(WindowsHook hookId, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// Removes a hook procedure installed in a hook chain.
    /// </summary>
    /// <param name="hhk">A handle to the hook to be removed.</param>
    /// <returns>A nonzero value if the function succeeds; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>
    /// Passes the hook information to the next hook procedure in the current hook chain.
    /// </summary>
    /// <param name="hhk">This parameter is ignored.</param>
    /// <param name="nCode">The hook code passed to the current hook procedure.</param>
    /// <param name="wParam">The <paramref name="wParam"/> value passed to the current hook procedure.</param>
    /// <param name="lParam">The <paramref name="lParam"/> value passed to the current hook procedure.</param>
    /// <returns>The value returned by the next hook procedure in the chain.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Adds the specified window to the chain of clipboard viewers.
    /// </summary>
    /// <param name="hWndNewViewer">A handle to the window to be added to the clipboard chain.</param>
    /// <returns>If successful, the handle to the next window in the clipboard viewer chain; otherwise zero.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr SetClipboardViewer(WindowHandle hWndNewViewer);

    /// <summary>
    /// Removes a specified window from the chain of clipboard viewers.
    /// </summary>
    /// <param name="hWndRemove">A handle to the window to be removed from the chain.</param>
    /// <param name="hWndNewNext">A handle to the window that follows the window being removed in the clipboard viewer chain.</param>
    /// <returns>
    /// The result of passing <see cref="WindowMessage.ChangeClipboardChain"/> to the windows in the clipboard viewer chain.
    /// </returns>
    [LibraryImport(LIBRARY_NAME)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool ChangeClipboardChain(WindowHandle hWndRemove, IntPtr hWndNewNext);

    /// <summary>
    /// Retrieves information about the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="nIndex">The zero-based offset to the value to be retrieved.</param>
    /// <returns>The requested value if successful; otherwise, zero.</returns>
    /// <remarks>This should only ever be called from a 32-bit machine.</remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "GetWindowLong")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial IntPtr GetWindowLongPtr32(WindowHandle hWnd, int nIndex);

    /// <summary>
    /// Retrieves information about the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="nIndex">The zero-based offset to the value to be retrieved.</param>
    /// <returns>The requested value if successful; otherwise, zero.</returns>
    /// <remarks>This should only ever be called from a 64-bit machine.</remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "GetWindowLongPtrW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial IntPtr GetWindowLongPtr64(WindowHandle hWnd, int nIndex);

    /// <summary>
    /// Changes an attribute of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="nIndex">The zero-based offset to the value to be set.</param>
    /// <param name="dwNewLong">The replacement value.</param>
    /// <returns>The previous value of the specified offset if successful; otherwise, zero.</returns>
    /// <remarks>This should only ever be called from a 32-bit machine.</remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "SetWindowLong")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial IntPtr SetWindowLongPtr32(WindowHandle hWnd, int nIndex, IntPtr dwNewLong);

    /// <summary>
    /// Changes an attribute of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="nIndex">The zero-based offset to the value to be set.</param>
    /// <param name="dwNewLong">The replacement value.</param>
    /// <returns>The previous value of the specified offset if successful; otherwise, zero.</returns>
    /// <remarks>This should only ever be called from a 64-bit machine.</remarks>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "SetWindowLongPtrW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial IntPtr SetWindowLongPtr64(WindowHandle hWnd, int nIndex, IntPtr dwNewLong);
}