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
/// Specifies flags that indicate a display device's state.
/// </summary>
[Flags]
internal enum DisplayDeviceStateFlags
{
    /// <summary>
    /// The device is presented as being "on".
    /// </summary>
    Active = 0x1,
    /// <summary>
    /// The primary desktop is on the device.
    /// </summary>
    Primary = 0x4,
    /// <summary>
    /// The device is used to mirror application drawing for remoting or other purposes.
    /// </summary>
    MirroringDriver = 0x8,
    /// <summary>
    /// The device is VGA compatible.
    /// </summary>
    VgaCompatible = 0x10,
    /// <summary>
    /// The device is removable.
    /// </summary>
    Removable = 0x20,
    /// <summary>
    /// The device has unsafe modes enabled.
    /// </summary>
    UnsafeModesOn = 0x80000,
    /// <summary>
    /// The device has more display modes than its output devices support.
    /// </summary>
    ModesPruned = 0x8000000
}
