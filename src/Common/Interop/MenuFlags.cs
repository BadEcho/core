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

namespace BadEcho.Interop;

/// <summary>
/// Specifies flags that control the appearance and behavior of a menu item.
/// </summary>
[Flags]
internal enum MenuFlags
{
    /// <summary>
    /// The menu item is enabled.
    /// </summary>
    Enabled = 0,
    /// <summary>
    /// The menu item is a text string.
    /// </summary>
    String = Enabled,
    /// <summary>
    /// The menu item is disabled and grayed out so that it cannot be selected.
    /// </summary>
    Grayed = 0x1,
    /// <summary>
    /// The menu item is disabled without graying it out so that it cannot be selected.
    /// </summary>
    Disabled = 0x2,
    /// <summary>
    /// The menu item is a bitmap.
    /// </summary>
    Bitmap = 0x4,
    /// <summary>
    /// The menu item has a check mark placed next to it.
    /// </summary>
    Checked = 0x8,
    /// <summary>
    /// The menu item opens a drop-down menu or submenu.
    /// </summary>
    Popup = 0x10,
    /// <summary>
    /// The menu item separates two columns with a vertical line; for menu bars, this behaves the same as <see cref="MenuBreak"/>.
    /// </summary>
    MenuBarBreak = 0x20,
    /// <summary>
    /// The menu item separates two columns without a vertical line; for menu bars, the item is placed on a new line.
    /// </summary>
    MenuBreak = 0x40,
    /// <summary>
    /// The menu item is an owner-drawn item.
    /// </summary>
    OwnerDraw = 0x100,
    /// <summary>
    /// The menu item is a horizontal dividing line.
    /// </summary>
    Separator = 0x800
}
