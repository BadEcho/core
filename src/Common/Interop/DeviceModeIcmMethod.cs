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
/// Specifies how ICM is handled for a device.
/// </summary>
internal enum DeviceModeIcmMethod : uint
{
    /// <summary>
    /// The manner in which ICM is handled is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Specifies that ICM is disabled.
    /// </summary>
    None,
    /// <summary>
    /// Specifies that ICM is handled by Windows.
    /// </summary>
    System,
    /// <summary>
    /// Specifies that ICM is handled by the device driver.
    /// </summary>
    Driver,
    /// <summary>
    /// Specifies that ICM is handled by the destination device.
    /// </summary>
    Device
}
