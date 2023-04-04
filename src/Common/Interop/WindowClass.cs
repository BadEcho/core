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

namespace BadEcho.Interop;

/// <summary>
/// Provides window class information.
/// </summary>
internal sealed class WindowClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClass"/> class.
    /// </summary>
    /// <param name="windowProc">The window procedure delegate.</param>
    public WindowClass(WindowProc windowProc)
    {
        Require.NotNull(windowProc, nameof(windowProc));

        WindowProc = windowProc;
    }

    /// <summary>
    /// The window procedure delegate.
    /// </summary>
    public WindowProc WindowProc
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