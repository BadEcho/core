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
/// Specifies flags that provide additional information when modifying the taskbar's status area.
/// </summary>
[Flags]
internal enum NotifyIconFlags
{
    /// <summary>
    /// The <see cref="NotifyIconData.CallbackMessage"/> member is valid.
    /// </summary>
    Message = 0x1,
    /// <summary>
    /// The <see cref="NotifyIconData.Icon"/> member is valid.
    /// </summary>
    Icon = 0x2,
    /// <summary>
    /// The <see cref="NotifyIconData.Tip"/> member is valid.
    /// </summary>
    Tip = 0x4,
    /// <summary>
    /// The <see cref="NotifyIconData.State"/> and <see cref="NotifyIconData.StateMask"/> members are valid.
    /// </summary>
    State = 0x8,
    /// <summary>
    /// The <see cref="NotifyIconData.Info"/>, <see cref="NotifyIconData.InfoTitle"/>,
    /// <see cref="NotifyIconData.InfoFlags"/>, and <see cref="NotifyIconData.TimeoutOrVersion"/> members are valid.
    /// </summary>
    Info = 0x10,
    /// <summary>
    /// The <see cref="NotifyIconData.Id"/> member is valid.
    /// </summary>
    Guid = 0x20,
    /// <summary>
    /// If the balloon notification cannot be displayed immediately, discard it.
    /// </summary>
    Realtime = 0x40,
    /// <summary>
    /// Use the standard tooltip.
    /// </summary>
    ShowTip = 0x80
}
