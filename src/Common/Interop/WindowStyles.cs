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
/// Specifies styling configurations for windows that (for the most part) cannot be modified after the window has
/// been created.
/// </summary>
[Flags]
public enum WindowStyles : uint
{
    /// <summary>
    /// The window excludes the area occupied by child windows when drawing occurs within it.
    /// </summary>
    ClipChildren = 0x02000000,
    /// <summary>
    /// The window is a child window.
    /// </summary>
    Child = 0x40000000,
    /// <summary>
    /// The window is a pop-up window.
    /// </summary>
    Popup = 0x80000000
}
