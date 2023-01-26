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

using System.Windows.Interop;
using BadEcho.Presentation.Properties;
using BadEcho.Interop;

namespace BadEcho.Presentation.Windows;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a window created by WPF.
/// </summary>
public sealed class PresentationWindowWrapper : IWindowWrapper
{
    private readonly Dictionary<WindowHookProc, HwndSourceHook> _hookMapper = new();
    private readonly HwndSource _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="PresentationWindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle for a window created by WPF.</param>
    public PresentationWindowWrapper(IntPtr handle)
    {
        var source = HwndSource.FromHwnd(handle);
        
        _source = source
            ?? throw new ArgumentException(Strings.WindowNotPresentation, nameof(handle));
        
        Handle = new WindowHandle(handle, false);
    }

    /// <inheritdoc/>
    public WindowHandle Handle
    { get; }

    /// <inheritdoc/>
    public void AddHook(WindowHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        if (_hookMapper.ContainsKey(hook))
            throw new ArgumentException(Strings.PresentationWindowWrapperDuplicateHook, nameof(hook));

        _hookMapper.Add(hook, SourceHook);
            
        _source.AddHook(SourceHook);

        IntPtr SourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return hook(hWnd, (uint)msg, wParam, lParam, ref handled);
        }
    }

    /// <inheritdoc/>
    public void RemoveHook(WindowHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        if (!_hookMapper.ContainsKey(hook))
            return;

        _source.RemoveHook(_hookMapper[hook]);
    }
}