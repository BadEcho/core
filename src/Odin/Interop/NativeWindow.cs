//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides an encapsulation of a Win32 window, providing information of interest as well as offering ways to manipulate
    /// said window.
    /// </summary>
    public sealed class NativeWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeWindow"/> class.
        /// </summary>
        /// <param name="handle">The handle to the window.</param>
        public NativeWindow(IntPtr handle)
        {
            Handle = handle;

            if (!User32.GetWindowRect(handle, out RECT rect))
                throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();

            Left = rect.Left;
            Top = rect.Top;
            Width = rect.Width;
            Height = rect.Height;
        }

        /// <summary>
        /// Gets the handle to the window.
        /// </summary>
        public IntPtr Handle
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
        /// as transparent (at least in WPF's case), is not sufficient to create a true overlay, as the window can still grab focus,
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
    }
}