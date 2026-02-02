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

namespace BadEcho.Interop.Audio;

/// <summary>
/// Specifies audio endpoint device states.
/// </summary>
[Flags]
public enum DeviceState
{
    /// <summary>
    /// The device is active.
    /// </summary>
    Active = 0x1,
    /// <summary>
    /// The device is disabled.
    /// </summary>
    Disabled = 0x2,
    /// <summary>
    /// The device is not present because the audio adapter that connects to the device
    /// has been removed from the system.
    /// </summary>
    NotPresent = 0x4,
    /// <summary>
    /// The device is unplugged.
    /// </summary>
    Unplugged = 0x8,
    /// <summary>
    /// Includes devices in all states.
    /// </summary>
    All = 0xF
}
