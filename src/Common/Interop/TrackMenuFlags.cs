//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies flags that control the position, animation, and other behavior of an opened menu.
/// </summary>
[Flags]
internal enum TrackMenuFlags
{
    /// <summary>
    /// Positions the menu so that its left side is aligned with a specified x-coordinate.
    /// </summary>
    LeftAlign = 0,
    /// <summary>
    /// The menu's items can be selected with either the left or right mouse buttons.
    /// </summary>
    RightButton = 0x2,
    /// <summary>
    /// Centers the menu horizontally relative to a specified x-coordinate.
    /// </summary>
    CenterAlign = 0x4,
    /// <summary>
    /// Positions the menu so that its right side is aligned with a specified x-coordinate.
    /// </summary>
    RightAlign = 0x8,
    /// <summary>
    /// Centers the menu vertically relative to a specified y-coordinate.
    /// </summary>
    VerticallyCenteredAlign = 0x10,
    /// <summary>
    /// Positions the menu so that its bottom side is aligned with a specified y-coordinate.
    /// </summary>
    BottomAlign = 0x20,
    /// <summary>
    /// The menu does not send notification messages when the user clicks a menu item.
    /// </summary>
    NoNotify = 0x80,
    /// <summary>
    /// The <see cref="User32.TrackPopupMenu"/> function returns the menu item identifier of the user's
    /// selection in the return value.
    /// </summary>
    ReturnCommand = 0x100,
    /// <summary>
    /// The menu is animated from left to right.
    /// </summary>
    AnimateLeftToRight = 0x400,
    /// <summary>
    /// The menu is animated from right to left.
    /// </summary>
    AnimateRightToLeft = 0x800,
    /// <summary>
    /// The menu is animated from top to bottom.
    /// </summary>
    AnimateTopToBottom = 0x1000,
    /// <summary>
    /// The menu is animated from bottom to top.
    /// </summary>
    AnimateBottomToTop = 0x2000,
    /// <summary>
    /// The menu is not animated.
    /// </summary>
    NotAnimated = 0x4000
}