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

using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides information about a display device.
/// </summary>
[NativeMarshalling(typeof(DisplayDeviceMarshaller))]
internal sealed class DisplayDevice
{
    /// <summary>
    /// Gets or sets the adapter or monitor device name.
    /// </summary>
    public string? Name
    { get; set; }

    /// <summary>
    /// Gets or sets the device context string, typically a description of the display adapter or monitor.
    /// </summary>
    public string? Context
    { get; set; }

    /// <summary>
    /// Gets or sets flags that indicating the state of the display device.
    /// </summary>
    public DisplayDeviceStateFlags Flags
    { get; set; }

    /// <summary>
    /// Gets or sets the device interface name, usable with SetupAPI functions.
    /// </summary>
    public string? Id
    { get; set; }
    
    /// <summary>
    /// Gets or sets the registry key containing the settings for this display device.
    /// </summary>
    public string? Key
    { get; set; }
}
