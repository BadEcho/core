//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Interop;

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
    /// Number of pixels per logical inch along the screen width.
    /// </summary>
    PpiWidth = 88,
    /// <summary>
    /// Number of pixels per logical inch along the screen height.
    /// </summary>
    PpiHeight = 90
}