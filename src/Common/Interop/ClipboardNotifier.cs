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

namespace BadEcho.Interop;

/// <summary>
/// Provides a notifier of global clipboard content changes.
/// </summary>
public sealed class ClipboardNotifier
{
    private readonly IWindowWrapper _windowWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardNotifier"/> class.
    /// </summary>
    /// <param name="windowWrapper">A wrapper around the window that will receive clipboard-related messages.</param>
    public ClipboardNotifier(IWindowWrapper windowWrapper)
    {
        Require.NotNull(windowWrapper, nameof(windowWrapper));

        _windowWrapper = windowWrapper;

        if (!User32.AddClipboardFormatListener(_windowWrapper.Handle))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        _windowWrapper.AddHook(WndProc);
    }

    /// <summary>
    /// Occurs when the contents of the clipboard have changed.
    /// </summary>
    public event EventHandler? ClipboardChanged; 

    private IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = (WindowMessage) msg;

        switch (message)
        {
            case WindowMessage.ClipboardUpdate:
                ClipboardChanged?.Invoke(this, EventArgs.Empty);
                handled = true;

                break;

            case WindowMessage.Destroy:
                if (!User32.RemoveClipboardFormatListener(_windowWrapper.Handle))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                break;
        }

        return IntPtr.Zero;
    }
}