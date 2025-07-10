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
/// Specifies flags that modify the behavior and appearance of a balloon notification.
/// </summary>
[Flags]
internal enum NotifyIconInfoFlags
{
    /// <summary>
    /// No icon.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// An information icon.
    /// </summary>
    Info = 0x1,
    /// <summary>
    /// A warning icon.
    /// </summary>
    Warning = 0x2,
    /// <summary>
    /// An error icon.
    /// </summary>
    Error = 0x3,
    /// <summary>
    /// Use the icon identified in <see cref="NotifyIconData.BalloonIcon"/> as the notification balloon's title icon.
    /// </summary>
    User = 0x4,
    /// <summary>
    /// Do not play the associated sound.
    /// </summary>
    NoSound = 0x10,
    /// <summary>
    /// Reserved.
    /// </summary>
    IconMask = 0xF,
    /// <summary>
    /// The large version of the icon should be used as the notification icon.
    /// </summary>
    LargeIcon = 0x20,
    /// <summary>
    /// Do not display the balloon notification if the current user is in "quiet time".
    /// </summary>
    RespectQuietTime = 0x80
}
