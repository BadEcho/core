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

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using BadEcho.Extensions;
using BadEcho.Logging;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides an encapsulation of a Win32 window, providing information of interest as well as offering ways to manipulate
/// said window.
/// </summary>
public sealed class NativeWindow
{
    private readonly List<int> _hotKeyIds = [];

    private bool _displayingWithoutFlicker;
    private bool _ignoreSizeChanges;
    
    private WindowStyles _displayStyles;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeWindow"/> class.
    /// </summary>
    /// <param name="windowWrapper">A wrapper around a window and the messages it receives.</param>
    public NativeWindow(WindowWrapper windowWrapper)
    {
        Require.NotNull(windowWrapper, nameof(windowWrapper));

        Handle = windowWrapper.Handle;
        
        if (!User32.GetWindowRect(Handle, out RECT rect))
            throw ((ResultHandle)Marshal.GetHRForLastWin32Error()).GetException();
        
        windowWrapper.AddCallback(WindowProcedure);

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
    { get; private set; }

    /// <summary>
    /// Gets the position of the window's top edge.
    /// </summary>
    public int Top
    { get; private set; }

    /// <summary>
    /// Gets the width of the window.
    /// </summary>
    public int Width
    { get; private set; }

    /// <summary>
    /// Gets the height of the window.
    /// </summary>
    public int Height
    { get; private set; }
    
    /// <summary>
    /// Gets the bounds of the caption button area for this window.
    /// </summary>
    public unsafe Rectangle CaptionButtonBounds
    {
        get
        {
            ResultHandle hResult = DesktopWindowManager.DwmGetWindowAttribute(Handle,
                                                                              DwmWindowAttribute.CaptionButtonBounds,
                                                                              out RECT bounds,
                                                                              sizeof(RECT));
            if (hResult == ResultHandle.Failure) 
                Logger.Error(Strings.WindowCaptionBoundsFailure, hResult.GetException());

            return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        }
    }

    /// <summary>
    /// Gets a value indicating if techniques intended to eliminate the possibility of a flickering background during the initial
    /// display of the window should be employed.
    /// </summary>
    public bool DisplayWithoutFlicker
    { get; init; }

    /// <summary>
    /// Gets or sets a value indicating if the window's context menu (normally opened by left-clicking the top-left corner of the window
    /// where its icon normally is, or by Alt + Spacebar) should be disabled.
    /// </summary>
    public bool DisableContextMenu 
    { get; set; }

    /// <summary>
    /// Applies a brush of the specified color as the background for this window's class.
    /// </summary>
    /// <param name="r">The intensity of the red color.</param>
    /// <param name="g">The intensity of the green color.</param>
    /// <param name="b">The intensity of the blue color.</param>
    /// <remarks>
    /// After changing the class background, window instances must have their client areas invalidated in order for the new
    /// background color to be painted. This can be done by invoking <see cref="Invalidate"/>.
    /// </remarks>
    public void ChangeClassBackground(byte r, byte g, byte b)
    {   // Win32 COLORREF values use the BGR format.
        int newBrushColor = (b << 16) | (g << 8) | r;
        
        IntPtr newBrush = Gdi32.CreateSolidBrush(newBrushColor);
        IntPtr oldBrush = User32.SetClassLongPtr(Handle, WindowClassAttribute.Background, newBrush);
        
        Gdi32.DeleteObject(oldBrush);
    }

    /// <summary>
    /// Invalidates the client area of the window, causing it to eventually be redrawn.
    /// </summary>
    public unsafe void Invalidate()
        => User32.InvalidateRect(Handle, null, true);

    /// <summary>
    /// Sets the windows extended style information so that said window acts as transparent overlay which all input passes through.
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
    /// Sets the windows style information so that said window is stripped of its title bar.
    /// </summary>
    public void RemoveTitleBar()
    {
        var style = (WindowStyles) User32.GetWindowLongPtr(Handle, WindowAttribute.Style);

        style &= ~(WindowStyles.Caption | WindowStyles.SystemMenu);

        User32.SetWindowLongPtr(Handle,
                                WindowAttribute.ExtendedStyle,
                                new IntPtr((int) style));
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

    private ProcedureResult WindowProcedure(IntPtr hWnd, uint msg, nint wParam, nint lParam)
    {
        var lResult = IntPtr.Zero;
        var message = (WindowMessage) msg;
        bool handled = false;

        switch (message)
        {
            case WindowMessage.HotKey:
                int hotKeyId = wParam.ToInt32();

                if (_hotKeyIds.Contains(hotKeyId))
                    HotKeyPressed?.Invoke(this, new EventArgs<int>(hotKeyId));

                break;

            case WindowMessage.Size:
                if (!_ignoreSizeChanges)
                {
                    Width = (int) lParam & 0xFFFF;
                    Height = (int) lParam >> 16;
                }

                break;

            case WindowMessage.EraseBackground:
                if (_displayingWithoutFlicker)
                {
                    Display display = Display.FromWindow(Handle);
                    int displayStyles = unchecked((int) _displayStyles);

                    User32.SetWindowLongPtr(Handle, WindowAttribute.Style, displayStyles);

                    // Prevent recursive handling of this message.
                    _displayingWithoutFlicker = false;

                    // We account for the invisible borders that exist around most windows for purpose of mouse cursor grabs.
                    // This becomes important if we're dealing with "fullscreen" bordered windows (it'll extend onto other displays
                    // without this adjustment). 
                    int centeredWidth = display.WorkingArea.Width / 2 - Width / 2 - 9;
                    int centeredHeight = display.WorkingArea.Height / 2 - Height / 2;

                    const WindowPositionFlags uFlags = WindowPositionFlags.NoRedraw
                                                       | WindowPositionFlags.ShowWindow
                                                       | WindowPositionFlags.NoSendChanging;

                    bool resized = User32.SetWindowPos(Handle,
                                                       IntPtr.Zero,
                                                       centeredWidth,
                                                       centeredHeight,
                                                       Width,
                                                       Height,
                                                       uFlags);
                    if (!resized)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    
                    handled = true;
                    lResult = new IntPtr(1);
                }

                break;

            case WindowMessage.ShowWindow when wParam == 1: // Ignore windows becoming hidden
                if (DisplayWithoutFlicker)
                {   // Window is about to be shown, and we're configured to make a best-attempt at preventing any initial flickering.
                    if (!User32.GetWindowRect(Handle, out RECT rect))
                        throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();

                    // The Windows OS has an ancient flaw where a window w/ a title bar, configured to be visible immediately upon
                    // creation, will briefly flash white as it is appearing before being painted with whatever background brush it uses.
                    // One can observe this behavior using all sorts of programs, even Chrome will briefly attack our sight with bright
                    // white its window gets painted.

                    // If a window lacks a title bar (i.e., it was created with the WS_POPUP style) then it will not flash. However, if
                    // a window is set to display upon creation via WS_VISIBLE and was configured to have a title bar, then it is too
                    // late. The only way to shield the abrasive white flicker is to shrink the window size to a pixel in both dimensions
                    // and remove the title bar, causing it to be essentially invisible. After it displays we can resize it, and its appearance
                    // will be much more smooth.

                    // We first back up the initial dimensions of the window. Depending on the API that was used to create the window
                    // (i.e., Win32 directory, SDL, etc.), the size characteristics of our window, even at this point in time in which
                    // the window is about to be shown, may not be finalized. To account for this, we monitor for changes in size during
                    // our display operation.
                    Width = rect.Width;
                    Height = rect.Height;
                    _displayingWithoutFlicker = true;

                    // Strip the title bar, shredding all evidence of the protective measure being taken to shield our fragile eyes from
                    // such deviance in window behavior.
                    const int popupStyle = unchecked((int) WindowStyles.Popup);

                    _displayStyles
                        = (WindowStyles) User32.SetWindowLongPtr(Handle, WindowAttribute.Style, popupStyle);

                    // We can't set the width and length to 0...they have to be at least 1. A single pixel window with no title bar is basically
                    // invisible. We temporarily disable the monitoring of size in our callback so that we don't update our size properties with bunk values.
                    _ignoreSizeChanges = true;

                    bool resized = User32.SetWindowPos(Handle,
                                                       IntPtr.Zero,
                                                       0,
                                                       0,
                                                       1,
                                                       1,
                                                       WindowPositionFlags.NoMove | WindowPositionFlags.NoActivate);
                    if (!resized)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    _ignoreSizeChanges = false;
                }

                break;

            case WindowMessage.SystemCommand:
                if (DisableContextMenu)
                {   // The lower 4 bits of wParam are for internal use and should be disregarded.
                    var systemCommand = (SystemCommand) (wParam.ToInt32() & 0xFFF0);

                    if (systemCommand is SystemCommand.MouseMenu or SystemCommand.KeyMenu)
                        handled = true;
                }

                break;
        }

        return new ProcedureResult(lResult, handled);
    }
}