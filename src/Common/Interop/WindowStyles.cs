//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies styling configurations for windows that (for the most part) cannot be modified after the window has
/// been created.
/// </summary>
[Flags]
public enum WindowStyles : uint
{
    /// <summary>
    /// The window is an overlapped window, meaning that it has a title bar and a border.
    /// </summary>
    Overlapped = 0x00000000,
    /// <summary>
    /// The window has a maximize button.
    /// </summary>
    MaximizeBox = 0x00010000,
    /// <summary>
    /// The window has a minimize button.
    /// </summary>
    MinimizeBox = 0x00020000,
    /// <summary>
    /// The window has a sizing border.
    /// </summary>
    ThickFrame = 0x00040000,
    /// <summary>
    /// The window has a horizontal scroll bar.
    /// </summary>
    HorizontalScrollBar = 0x00100000,
    /// <summary>
    /// The window has a vertical scroll bar.
    /// </summary>
    VerticalScrollBar = 0x00200000,
    /// <summary>
    /// The window has a window menu on its title bar.
    /// </summary>
    SystemMenu = 0x00080000,
    /// <summary>
    /// The window has a border of a style typically used with dialog boxes.
    /// </summary>
    DialogFrame = 0x00400000,
    /// <summary>
    /// The window has a thin-line border.
    /// </summary>
    Border = 0x00800000,
    /// <summary>
    /// The window is initially maximized.
    /// </summary>
    Maximized = 0x01000000,
    /// <summary>
    /// The window has a title bar and border.
    /// </summary>
    Caption = DialogFrame | Border,
    /// <summary>
    /// The window excludes the area occupied by child windows when drawing occurs within it.
    /// </summary>
    ClipChildren = 0x02000000,
    /// <summary>
    /// The children of this window are clipped relative to each other; that is, when a particular child window is being
    /// painted, all other overlapping child windows are clipped out of the window's update region.
    /// </summary>
    ClipSiblings = 0x04000000,
    /// <summary>
    /// The window is initially visible.
    /// </summary>
    Visible = 0x10000000,
    /// <summary>
    /// The window is initially minimized.
    /// </summary>
    Minimized = 0x20000000,
    /// <summary>
    /// The window is a child window.
    /// </summary>
    Child = 0x40000000,
    /// <summary>
    /// The window is a pop-up window.
    /// </summary>
    Popup = 0x80000000
}
