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
/// Provides settings for a printer or display device mode.
/// </summary>
[NativeMarshalling(typeof(DeviceModeMarshaller))]
internal sealed class DeviceMode
{
    /// <summary>
    /// Gets or sets the "friendly" name of the printer or display device.
    /// </summary>
    public string? DeviceName
    { get; set; }

    /// <summary>
    /// Gets or sets the version number of the initialization data specification on which this type is based.
    /// </summary>
    public ushort SpecificationVersion
    { get; set; }

    /// <summary>
    /// Gets or sets the driver version number assigned by the driver developer.
    /// </summary>
    public ushort DriverVersion
    { get; set; }

    /// <summary>
    /// Gets or sets the number of bytes of additional private driver-specific data that follow the
    /// public members of the unmanaged equivalent of this type.
    /// </summary>
    public ushort DriverExtra
    { get; set; }

    /// <summary>
    /// Gets or sets the flags specifying the members of this type that have been initialized.
    /// </summary>
    public DeviceModeFields Fields
    { get; set; }

    /// <summary>
    /// Gets or sets the positional x-coordinate of the display device.
    /// </summary>
    public int PositionX
    { get; set; }

    /// <summary>
    /// Gets or sets the positional y-coordinate of the display device.
    /// </summary>
    public int PositionY
    { get; set; }

    /// <summary>
    /// Gets or sets the orientation at which images should be presented.
    /// </summary>
    public DeviceModeDisplayOrientation DisplayOrientation
    { get; set; }

    /// <summary>
    /// Gets or sets how the display presents a low-resolution mode on a higher-resolution display.
    /// </summary>
    public DeviceModeDisplayFixedOutput DisplayFixedOutput
    { get; set; }

    /// <summary>
    /// Gets or sets the color mode of the device.
    /// </summary>
    public DeviceModeColor Color
    { get; set; }

    /// <summary>
    /// Gets or sets whether single- or double-sided printing is used.
    /// </summary>
    public DeviceModeDuplex Duplex
    { get; set; }

    /// <summary>
    /// Gets or sets the y-resolution, in dots per inch, of the printer.
    /// </summary>
    public short YResolution
    { get; set; }

    /// <summary>
    /// Gets or sets how TrueType fonts should be printed.
    /// </summary>
    public DeviceModeTrueTypeOption TrueTypeOption
    { get; set; }

    /// <summary>
    /// Gets or sets whether collation should be used when printing multiple copies (0 to disable, 1 to enable).
    /// </summary>
    public short Collate
    { get; set; }

    /// <summary>
    /// Gets or sets the name of the form to use.
    /// </summary>
    public string? FormName
    { get; set; }

    /// <summary>
    /// Gets or sets the number of pixels per logical inch.
    /// </summary>
    public ushort LogPixels
    { get; set; }

    /// <summary>
    /// Gets or sets the color resolution, in bits per pixel, of the display device.
    /// </summary>
    public uint BitsPerPixel
    { get; set; }

    /// <summary>
    /// Gets or sets the width, in pixels, of the visible device surface.
    /// </summary>
    public uint Width
    { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of the visible device surface.
    /// </summary>
    public uint Height
    { get; set; }

    /// <summary>
    /// Gets or sets the device's display mode.
    /// </summary>
    public DeviceModeDisplayFlags DisplayFlags
    { get; set; }

    /// <summary>
    /// Gets or sets the frequency, in hertz, of the display device.
    /// </summary>
    public uint DisplayFrequency
    { get; set; }

    /// <summary>
    /// Gets or sets how ICM is handled.
    /// </summary>
    public DeviceModeIcmMethod IcmMethod
    { get; set; }

    /// <summary>
    /// Gets or sets the color matching method that should be used by default.
    /// </summary>
    public DeviceModeIcmIntent IcmIntent
    { get; set; }

    /// <summary>
    /// Gets or sets the type of media being printed on.
    /// </summary>
    public DeviceModeMediaType MediaType
    { get; set; }

    /// <summary>
    /// Gets or sets how dithering is to be done.
    /// </summary>
    public DeviceModeDitherType DitherType
    { get; set; }
}
