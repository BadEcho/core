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

using System.Runtime.InteropServices;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides an encapsulation of a Win32 window, providing information of interest as well as offering ways to manipulate
/// said window.
/// </summary>
public sealed class NativeWindow
{
    private readonly List<int> _hotKeyIds = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeWindow"/> class.
    /// </summary>
    /// <param name="handle">The handle to the window.</param>
    /// <param name="windowWrapper">A wrapper around a window and the messages it receives.</param>
    public NativeWindow(IntPtr handle, IWindowWrapper windowWrapper)
    {
        Require.NotNull(windowWrapper, nameof(windowWrapper));

        Handle = new WindowHandle(handle, false);

        windowWrapper.AddHook(WndProc);

        if (!User32.GetWindowRect(Handle, out RECT rect))
            throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();

        Left = rect.Left;
        Top = rect.Top;
        Width = rect.Width;
        Height = rect.Height;
    }

    /// <summary>
    /// Occurs when a registered hot key has been pressed.
    /// </summary>
    public event EventHandler<EventArgs<int>>? HotKeyPressed;

    /// <summary>
    /// Gets the handle to the window.
    /// </summary>
    public WindowHandle Handle
    { get; }

    /// <summary>
    /// Gets the position of the window's left edge.
    /// </summary>
    public int Left
    { get; }

    /// <summary>
    /// Gets the position of the window's top edge.
    /// </summary>
    public int Top
    { get; }

    /// <summary>
    /// Gets the width of the window.
    /// </summary>
    public int Width
    { get; }

    /// <summary>
    /// Gets the height of the window.
    /// </summary>
    public int Height
    { get; }

    /// <summary>
    /// Sets the windows extended style information so that acts as transparent overlay which all input passes through.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some user interface frameworks, such as WPF, allow for the developer to set a window to transparent, which is something
    /// we may wish to do if we are interested in constructing a transparent overlay showing information. Simply setting a window
    /// as transparent (at least in the case of WPF), is not sufficient to create a true overlay, as the window can still grab focus,
    /// preventing a total click-through experience, especially if there are any controls on the window (which there is a pretty
    /// good chance of).
    /// </para>
    /// <para>
    /// This method can be used to make any window a completely transparent overlay for which all input events will simply pass through
    /// to the window beneath it. Note that this method will only make it transparent in terms of its behavior; additional means are
    /// required in order to make the window visually transparent.
    /// </para>
    /// </remarks>
    public void MakeOverlay()
    {
        var extendedStyle = (ExtendedWindowStyles) User32.GetWindowLongPtr(Handle, WindowAttribute.ExtendedStyle);

        // Check if the transparency style is already applied.
        if (extendedStyle.HasFlag(ExtendedWindowStyles.Transparent))
            return;

        extendedStyle |= ExtendedWindowStyles.Transparent;

        User32.SetWindowLongPtr(Handle,
                                WindowAttribute.ExtendedStyle,
                                new IntPtr((int) extendedStyle));
    }

    /// <summary>
    /// Defines a system-wide hot key for this window.
    /// </summary>
    /// <param name="id">The identifier of the hot key.</param>
    /// <param name="modifiers">The keys that must be pressed in combination with the specified virtual key.</param>
    /// <param name="key">The virtual-key code of the hot key.</param>
    public void RegisterHotKey(int id, ModifierKeys modifiers, VirtualKey key)
    {
        if (_hotKeyIds.Contains(id))
            throw new ArgumentException(Strings.WindowHotKeyDuplicateId.InvariantFormat(id), nameof(id));

        _hotKeyIds.Add(id);

        if (!User32.RegisterHotKey(Handle, id, modifiers, key))
            throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();
    }

    /// <summary>
    /// Frees a hot key previously registered for this window.
    /// </summary>
    /// <param name="id">The identifier of the hot key to be freed.</param>
    public void UnregisterHotKey(int id)
    {
        if (!User32.UnregisterHotKey(Handle, id))
            throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = (WindowMessage) msg;

        switch (message)
        {
            case WindowMessage.HotKey:
                int hotKeyId = wParam.ToInt32();

                if (_hotKeyIds.Contains(hotKeyId))
                    HotKeyPressed?.Invoke(this, new EventArgs<int>(hotKeyId));

                break;
        }

        return IntPtr.Zero;
    }
}