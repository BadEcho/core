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
/// Specifies a type of device-specific information that can be requested through <see cref="Gdi32.GetDeviceCaps"/>.
/// </summary>
public enum DeviceInformation
{
    /// <summary>
    /// The device driver version.
    /// </summary>
    DriverVersion = 0,
    /// <summary>
    /// Technically the number of adjacent color bits for each pixel; however, since <see cref="ColorPlanes"/> seems to typically
    /// always be 1, this is essentially the bits per pixel (or bit-depth, found by multiplying this by the number of planes).
    /// </summary>
    BitsPixel = 12,
    /// <summary>
    /// The number of color planes.
    /// </summary>
    ColorPlanes = 14,
    /// <summary>
    /// Number of pixels per logical inch along the screen width.
    /// </summary>
    PpiWidth = 88,
    /// <summary>
    /// Number of pixels per logical inch along the screen height.
    /// </summary>
    PpiHeight = 90
}