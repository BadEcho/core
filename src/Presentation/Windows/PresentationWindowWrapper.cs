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

using System.Windows.Interop;
using BadEcho.Fenestra.Properties;
using BadEcho.Interop;

namespace BadEcho.Fenestra.Windows;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a window created by WPF.
/// </summary>
public sealed class PresentationWindowWrapper : IWindowWrapper
{
    private readonly Dictionary<WindowProc, HwndSourceHook> _hookMapper = new();
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
    }

    /// <inheritdoc/>
    public void AddHook(WindowProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        if (_hookMapper.ContainsKey(hook))
            throw new ArgumentException(Strings.PresentationWindowWrapperDuplicateHook, nameof(hook));

        _hookMapper.Add(hook, SourceHook);
            
        _source.AddHook(SourceHook);

        IntPtr SourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool _)
        {
            return hook.Invoke(hWnd, (uint)msg, wParam, lParam);
        }
    }

    /// <inheritdoc/>
    public void RemoveHook(WindowProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        if (!_hookMapper.ContainsKey(hook))
            return;

        _source.RemoveHook(_hookMapper[hook]);
    }
}