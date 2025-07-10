﻿// -----------------------------------------------------------------------
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

namespace BadEcho.Interop;

/// <summary>
/// Provides window class information.
/// </summary>
internal sealed class WindowClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClass"/> class.
    /// </summary>
    /// <param name="wndProc">The window procedure delegate that will be invoked by the operating system.</param>
    public WindowClass(WNDPROC wndProc)
    {
        Require.NotNull(wndProc, nameof(wndProc));

        WndProc = wndProc;
    }

    /// <summary>
    /// Gets the window procedure delegate that will be invoked by the operating system.
    /// </summary>
    public WNDPROC WndProc
    { get; }

    /// <summary>
    /// Gets or sets the class style(s).
    /// </summary>
    public int Style
    { get; set; }

    /// <summary>
    /// Gets or sets the number of extra bytes to allocate following the window-class structure.
    /// </summary>
    public int ClassExtraBytes
    { get; set; }

    /// <summary>
    /// Gets or sets the number of extra bytes to allocate following the window instance.
    /// </summary>
    public int WindowExtraBytes
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the instance that contains the window procedure for the class.
    /// </summary>
    public IntPtr Instance
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the class icon.
    /// </summary>
    public IntPtr Icon
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the class cursor.
    /// </summary>
    public IntPtr Cursor
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the class background brush.
    /// </summary>
    public IntPtr BackgroundBrush
    { get; set; }

    /// <summary>
    /// Gets or sets the resource name of the class menu.
    /// </summary>
    public string? MenuName
    { get; set; }

    /// <summary>
    /// Gets or sets either the window class name or its unique atom.
    /// </summary>
    public string? ClassName
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to a small icon that is associated with the window class.
    /// </summary>
    public IntPtr SmallIcon
    { get; set; }
}