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
/// Specifies the <see cref="DeviceMode"/> fields to include as changes to a device's settings.
/// </summary>
[Flags]
internal enum DeviceModeFields
{
    /// <summary>
    /// Use the <see cref="DeviceMode.PositionX"/> and <see cref="DeviceMode.PositionY"/> values.
    /// </summary>
    Position = 0x20,
    /// <summary>
    /// Use the <see cref="DeviceMode.DisplayOrientation"/> value.
    /// </summary>
    DisplayOrientation = 0x80,
    /// <summary>
    /// Use the <see cref="DeviceMode.Color"/> value.
    /// </summary>
    Color = 0x800,
    /// <summary>
    /// Use the <see cref="DeviceMode.Duplex"/> value.
    /// </summary>
    Duplex = 0x1000,
    /// <summary>
    /// Use the <see cref="DeviceMode.YResolution"/> value.
    /// </summary>
    YResolution = 0x2000,
    /// <summary>
    /// Use the <see cref="DeviceMode.TrueTypeOption"/> value.
    /// </summary>
    TrueTypeOption = 0x4000,
    /// <summary>
    /// Use the <see cref="DeviceMode.Collate"/> value.
    /// </summary>
    Collate = 0x8000,
    /// <summary>
    /// Use the <see cref="DeviceMode.FormName"/> value.
    /// </summary>
    FormName = 0x10000,
    /// <summary>
    /// Use the <see cref="DeviceMode.LogPixels"/> value.
    /// </summary>
    LogPixels = 0x20000,
    /// <summary>
    /// Use the <see cref="DeviceMode.BitsPerPixel"/> value.
    /// </summary>
    BitsPerPixel = 0x40000,
    /// <summary>
    /// Use the <see cref="DeviceMode.Width"/> value.
    /// </summary>
    Width = 0x80000,
    /// <summary>
    /// Use the <see cref="DeviceMode.Height"/> value.
    /// </summary>
    Height = 0x100000,
    /// <summary>
    /// Use the <see cref="DeviceMode.DisplayFlags"/> value.
    /// </summary>
    DisplayFlags = 0x200000,
    /// <summary>
    /// Use the <see cref="DeviceMode.DisplayFrequency"/> value.
    /// </summary>
    DisplayFrequency = 0x400000,
    /// <summary>
    /// Use the <see cref="DeviceMode.IcmMethod"/> value.
    /// </summary>
    IcmMethod = 0x00800000,
    /// <summary>
    /// Use the <see cref="DeviceMode.IcmIntent"/> value.
    /// </summary>
    IcmIntent = 0x01000000,
    /// <summary>
    /// Use the <see cref="DeviceMode.MediaType"/> value.
    /// </summary>
    MediaType = 0x02000000,
    /// <summary>
    /// Use the <see cref="DeviceMode.DitherType"/> value.
    /// </summary>
    DitherType = 0x04000000,
    /// <summary>
    /// Use the <see cref="DeviceMode.DisplayFixedOutput"/> value.
    /// </summary>
    DisplayFixedOutput = 0x20000000
}
