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
/// Provides a custom marshaller for display device information.
/// </summary>
[CustomMarshaller(typeof(DisplayDevice), MarshalMode.ManagedToUnmanagedRef, typeof(DisplayDeviceMarshaller))]
internal static unsafe class DisplayDeviceMarshaller
{
    /// <summary>
    /// Converts a managed <see cref="DisplayDevice"/> instance to its unmanaged counterpart.
    /// </summary>
    /// <param name="managed">A managed instance of display device information.</param>
    /// <returns>A <see cref="DISPLAY_DEVICEW"/> equivalent of <c>managed</c>.</returns>
    public static DISPLAY_DEVICEW ConvertToUnmanaged(DisplayDevice managed)
    {
        Require.NotNull(managed, nameof(managed));
        
        // The structure is only written to, and never read from, unmanaged code.
        return new DISPLAY_DEVICEW
               {
                   cb = (uint) sizeof(DISPLAY_DEVICEW),
               };
    }

    /// <summary>
    /// Converts an unmanaged <see cref="DISPLAY_DEVICEW"/> instance to its managed counterpart.
    /// </summary>
    /// <param name="unmanaged">An unmanaged instance of display device information.</param>
    /// <returns>A <see cref="DisplayDevice"/> equivalent of <c>unmanaged</c>.</returns>
    public static DisplayDevice ConvertToManaged(DISPLAY_DEVICEW unmanaged)
    {
        return new DisplayDevice
               {
                   Name = new string(unmanaged.DeviceName),
                   Context = new string(unmanaged.DeviceString),
                   Flags = unmanaged.StateFlags,
                   Id = new string(unmanaged.DeviceId),
                   Key = new string(unmanaged.DeviceKey)
               };
    }

    /// <summary>
    /// Represents information about a display device.
    /// </summary>
    /// <suppressions>
    /// ReSharper disable InconsistentNaming
    /// </suppressions>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DISPLAY_DEVICEW
    {
        /// <summary>
        /// Size, in bytes, of this structure.
        /// </summary>
        public uint cb;
        /// <summary>
        /// A null-terminated string identifying the adapter or monitor device name.
        /// </summary>
        public fixed char szDeviceName[32];
        /// <summary>
        /// A null-terminated string specifying the device context string, typically a description of the
        /// display adapter or monitor.
        /// </summary>
        public fixed char szDeviceString[128];
        /// <summary>
        /// Device state flags.
        /// </summary>
        public DisplayDeviceStateFlags StateFlags;
        /// <summary>
        /// A null-terminated string specifying the device interface name, usable with SetupAPI functions.
        /// </summary>
        public fixed char szDeviceID[128];
        /// <summary>
        /// A null-terminated string specifying the registry key containing the settings for this display device.
        /// </summary>
        public fixed char szDeviceKey[128];

        /// <summary>
        /// Gets a string identifying the adapter or monitor device name.
        /// </summary>
        public readonly ReadOnlySpan<char> DeviceName
        {
            get
            {
                fixed (char* c = szDeviceName)
                {
                    return new Span<char>(c, 32).SliceAtFirstNull();
                }
            }
        }

        /// <summary>
        /// Gets a string specifying the device context, typically a description of the display adapter or monitor.
        /// </summary>
        public readonly ReadOnlySpan<char> DeviceString
        {
            get
            {
                fixed (char* c = szDeviceString)
                {
                    return new Span<char>(c, 128).SliceAtFirstNull();
                }
            }
        }

        /// <summary>
        /// Gets a string specifying the device interface name, usable with SetupAPI functions.
        /// </summary>
        public readonly ReadOnlySpan<char> DeviceId
        {
            get
            {
                fixed (char* c = szDeviceID)
                {
                    return new Span<char>(c, 128).SliceAtFirstNull();
                }
            }
        }

        /// <summary>
        /// Gets a string specifying the registry key containing the settings for this display device.
        /// </summary>
        public readonly ReadOnlySpan<char> DeviceKey
        {
            get
            {
                fixed (char* c = szDeviceKey)
                {
                    return new Span<char>(c, 128).SliceAtFirstNull();
                }
            }
        }
    }
}
