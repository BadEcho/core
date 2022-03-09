//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides window class information.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEX
{
    /// <summary>
    /// The size, in bytes, of this structure.
    /// </summary>
    [MarshalAs(UnmanagedType.U4)]
    public int cbSize;
    /// <summary>
    /// The class style(s).
    /// </summary>
    [MarshalAs(UnmanagedType.U4)]
    public int style;
    /// <summary>
    /// A pointer to the window procedure.
    /// </summary>
    public WindowProc lpfnWndProc;
    /// <summary>
    /// The number of extra bytes to allocate following the window-class structure.
    /// </summary>
    public int cbClsExtra;
    /// <summary>
    /// The number of extra bytes to allocate following the window instance.
    /// </summary>
    public int cbWndExtra;
    /// <summary>
    /// A handle ot the instance that contains the window procedure for the class.
    /// </summary>
    public IntPtr hInstance;
    /// <summary>
    /// A handle to the class icon.
    /// </summary>
    public IntPtr hIcon;
    /// <summary>
    /// A handle to the class cursor.
    /// </summary>
    public IntPtr hCursor;
    /// <summary>
    /// A handle to the class background brush.
    /// </summary>
    public IntPtr hbrBackground;
    /// <summary>
    /// Specifies the resource name of the class menu.
    /// </summary>
    public string lpszMenuName;
    /// <summary>
    /// Either specifies the window class name or is a unique atom.
    /// </summary>
    public string lpszClassName;
    /// <summary>
    /// A handle to a small icon that is associated with the window class.
    /// </summary>
    public IntPtr hIconSm;
}
