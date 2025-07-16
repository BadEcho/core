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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using BadEcho.Extensions;

namespace BadEcho.Interop;

/// <summary>
/// Provides a custom marshaller for printer or display device settings.
/// </summary>
[CustomMarshaller(typeof(DeviceMode), MarshalMode.ManagedToUnmanagedRef, typeof(DeviceModeMarshaller))]
internal static unsafe class DeviceModeMarshaller
{
    /// <summary>
    /// Converts a managed <see cref="DeviceMode"/> instance to its unmanaged counterpart.
    /// </summary>
    /// <param name="managed">A managed instance of printer or display device settings.</param>
    /// <returns>A <see cref="DEVMODEW"/> equivalent of <c>managed</c>.</returns>
    public static DEVMODEW ConvertToUnmanaged(DeviceMode managed)
    {
        Require.NotNull(managed, nameof(managed));

        return new DEVMODEW
               {
                   DeviceName = managed.DeviceName,
                   dmSpecVersion = managed.SpecificationVersion,
                   dmDriverVersion = managed.DriverVersion,
                   dmSize = (ushort) sizeof(DEVMODEW),
                   dmDriverExtra = managed.DriverExtra,
                   dmFields = managed.Fields,
                   dmPositionX = managed.PositionX,
                   dmPositionY = managed.PositionY,
                   dmDisplayOrientation = managed.DisplayOrientation,
                   dmDisplayFixedOutput = managed.DisplayFixedOutput,
                   dmColor = managed.Color,
                   dmDuplex = managed.Duplex,
                   dmYResolution = managed.YResolution,
                   dmTTOption = managed.TrueTypeOption,
                   dmCollate = managed.Collate,
                   FormName = managed.FormName,
                   dmLogPixels = managed.LogPixels,
                   dmBitsPerPel = managed.BitsPerPixel,
                   dmPelsWidth = managed.Width,
                   dmPelsHeight = managed.Height,
                   dmDisplayFlags = managed.DisplayFlags,
                   dmDisplayFrequency = managed.DisplayFrequency,
                   dmICMMethod = managed.IcmMethod,
                   dmICMIntent = managed.IcmIntent,
                   dmMediaType = managed.MediaType,
                   dmDitherType = managed.DitherType,
                   dmPanningWidth = 0,
                   dmPanningHeight = 0
               };
    }

    /// <summary>
    /// Converts an unmanaged <see cref="DEVMODEW"/> instance to its managed counterpart.
    /// </summary>
    /// <param name="unmanaged">An unmanaged instance of printer or display device settings.</param>
    /// <returns>A <see cref="DEVMODEW"/> equivalent of <c>unmanaged</c>.</returns>
    public static DeviceMode ConvertToManaged(DEVMODEW unmanaged)
    {
        return new DeviceMode
               {
                   DeviceName = new string(unmanaged.DeviceName),
                   SpecificationVersion = unmanaged.dmSpecVersion,
                   DriverVersion = unmanaged.dmDriverVersion,
                   DriverExtra = unmanaged.dmDriverExtra,
                   Fields = unmanaged.dmFields,
                   PositionX = unmanaged.dmPositionX,
                   PositionY = unmanaged.dmPositionY,
                   DisplayOrientation = unmanaged.dmDisplayOrientation,
                   DisplayFixedOutput = unmanaged.dmDisplayFixedOutput,
                   Color = unmanaged.dmColor,
                   Duplex = unmanaged.dmDuplex,
                   YResolution = unmanaged.dmYResolution,
                   TrueTypeOption = unmanaged.dmTTOption,
                   Collate = unmanaged.dmCollate,
                   FormName = new string(unmanaged.FormName),
                   LogPixels = unmanaged.dmLogPixels,
                   BitsPerPixel = unmanaged.dmBitsPerPel,
                   Width = unmanaged.dmPelsWidth,
                   Height = unmanaged.dmPelsHeight,
                   DisplayFlags = unmanaged.dmDisplayFlags,
                   DisplayFrequency = unmanaged.dmDisplayFrequency,
                   IcmMethod = unmanaged.dmICMMethod,
                   IcmIntent = unmanaged.dmICMIntent,
                   MediaType = unmanaged.dmMediaType,
                   DitherType = unmanaged.dmDitherType
               };
    }

    /// <summary>
    /// Represents settings for a printer or display device.
    /// </summary>
    /// <suppressions>
    /// ReSharper disable InconsistentNaming
    /// </suppressions>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    internal struct DEVMODEW
    {
        /// <summary>
        /// A null-terminated character array that specifies the "friendly" name of the printer or display device.
        /// </summary>
        public fixed char dmDeviceName[32];
        /// <summary>
        /// The version number of the initialization data specification on which this structure is based.
        /// </summary>
        public ushort dmSpecVersion;
        /// <summary>
        /// The driver version number assigned by the driver developer.
        /// </summary>
        public ushort dmDriverVersion;
        /// <summary>
        /// Specifies the size, in bytes, of this structure, not including any driver-specific data that might follow.
        /// </summary>
        public ushort dmSize;
        /// <summary>
        /// The number of bytes of additional private driver-specific data that follow the public members of this structure.
        /// </summary>
        public ushort dmDriverExtra;
        /// <summary>
        /// Flags specifying the members of this type that have been initialized.
        /// </summary>
        public DeviceModeFields dmFields;
        /// <summary>
        /// The positional x-coordinate of the display device.
        /// </summary>
        public int dmPositionX;
        /// <summary>
        /// The positional y-coordinate of the display device.
        /// </summary>
        public int dmPositionY;
        /// <summary>
        /// The orientation at which images should be presented.
        /// </summary>
        public DeviceModeDisplayOrientation dmDisplayOrientation;
        /// <summary>
        /// The orientation at which images should be presented.
        /// </summary>
        public DeviceModeDisplayFixedOutput dmDisplayFixedOutput;
        /// <summary>
        /// The color mode of the device.
        /// </summary>
        public DeviceModeColor dmColor;
        /// <summary>
        /// Specifies whether single- or double-sided printing is used.
        /// </summary>
        public DeviceModeDuplex dmDuplex;
        /// <summary>
        /// The y-resolution, in dots per inch, of the printer.
        /// </summary>
        public short dmYResolution;
        /// <summary>
        /// Specifies how TrueType fonts should be printed.
        /// </summary>
        public DeviceModeTrueTypeOption dmTTOption;
        /// <summary>
        /// Specifies whether collation should be used when printing multiple copies (0 to disable, 1 to enable).
        /// </summary>
        public short dmCollate;
        /// <summary>
        /// A null-terminated character array that specifies the name of the form to use.
        /// </summary>
        public fixed char dmFormName[32];
        /// <summary>
        /// The number of pixels per logical inch.
        /// </summary>
        public ushort dmLogPixels;
        /// <summary>
        /// The color resolution, in bits per pixel, of the display device.
        /// </summary>
        public uint dmBitsPerPel;
        /// <summary>
        /// The width, in pixels, of the visible device surface.
        /// </summary>
        public uint dmPelsWidth;
        /// <summary>
        /// The height, in pixels, of the visible device surface.
        /// </summary>
        public uint dmPelsHeight;
        /// <summary>
        /// The device's display mode.
        /// </summary>
        public DeviceModeDisplayFlags dmDisplayFlags;
        /// <summary>
        /// The frequency, in hertz, of the display device.
        /// </summary>
        public uint dmDisplayFrequency;
        /// <summary>
        /// Specifies how ICM is handled.
        /// </summary>
        public DeviceModeIcmMethod dmICMMethod;
        /// <summary>
        /// Specifies the color matching method that should be used by default.
        /// </summary>
        public DeviceModeIcmIntent dmICMIntent;
        /// <summary>
        /// Specifies the type of media being printed on.
        /// </summary>
        public DeviceModeMediaType dmMediaType;
        /// <summary>
        /// Specifies how dithering is to be done.
        /// </summary>
        public DeviceModeDitherType dmDitherType;
        /// <summary>
        /// Not used; must be zero.
        /// </summary>
        public uint dmReserved1;
        /// <summary>
        /// Not used; must be zero.
        /// </summary>
        public uint dmReserved2;
        /// <summary>
        /// This member must be zero.
        /// </summary>
        public uint dmPanningWidth;
        /// <summary>
        /// This member must be zero. Ask no questions!
        /// </summary>
        public uint dmPanningHeight;
        
        /// <summary>
        /// Gets or sets a string that specifies the "friendly" name of the printer or display device.
        /// </summary>
        public ReadOnlySpan<char> DeviceName
        {
            readonly get => SzDeviceName.SliceAtFirstNull();
            set => value.CopyToAndTerminate(SzDeviceName);
        }
        
        /// <summary>
        /// Gets or sets a string that specifies the name of the form to use.
        /// </summary>
        public ReadOnlySpan<char> FormName
        {
            readonly get => SzFormName.SliceAtFirstNull();
            set => value.CopyToAndTerminate(SzFormName);
        }

        /// <summary>
        /// Gets or sets a null-terminated string that specifies the "friendly" name of the printer or display device.
        /// </summary>
        private readonly Span<char> SzDeviceName
        {
            get
            {
                fixed (char* c = dmDeviceName)
                {
                    return new Span<char>(c, 32);
                }
            }
        }

        /// <summary>
        /// Gets or sets a null-terminated string that specifies the name of the form to use.
        /// </summary>
        private readonly Span<char> SzFormName
        {
            get
            {
                fixed (char* c = dmFormName)
                {
                    return new Span<char>(c, 32);
                }
            }
        }
    }
}
