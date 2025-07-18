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

using BadEcho.Interop;

namespace BadEcho.Tests.Interop;

internal static class TestWindow
{
    public static WindowHandle Create(string className)
    {
        RegisterClass(className);
        IntPtr hInstance = Kernel32.GetModuleHandle(null);

        unsafe
        {
            return User32.CreateWindowEx(0,
                                         className,
                                         string.Empty,
                                         0,
                                         0,
                                         0,
                                         0,
                                         0,
                                         IntPtr.Zero,
                                         IntPtr.Zero,
                                         hInstance,
                                         null);
        }
    }
    
    internal static ushort RegisterClass(string className)
    {
        var windowClass = new WindowClass(WndProc);

        IntPtr hInstance = Kernel32.GetModuleHandle(null);
        
        windowClass.Style = 0;
        windowClass.ClassExtraBytes = 0;
        windowClass.WindowExtraBytes = 0;
        windowClass.Instance = hInstance;
        windowClass.Icon = IntPtr.Zero;
        windowClass.Cursor = IntPtr.Zero;
        windowClass.BackgroundBrush = IntPtr.Zero;
        windowClass.MenuName = string.Empty;
        windowClass.Name = className;
        windowClass.SmallIcon = IntPtr.Zero;

        return User32.RegisterClassEx(ref windowClass);
    }

    internal static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) 
        => new(1);
}
