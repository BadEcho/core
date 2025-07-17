﻿// -----------------------------------------------------------------------
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
using BadEcho.Extensions;

namespace BadEcho.Interop;

/// <summary>
/// Represents a display monitor.
/// </summary>
/// <remarks>
/// In order to create a <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions, one must make
/// use of the <see cref="CreateWritable"/> method.
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
internal unsafe struct MONITORINFOEX
{
    /// <summary>
    /// The size, in bytes, of the structure.
    /// </summary>
    public uint cbSize;

    /// <summary>
    /// The display monitor rectangle, expressed in virtual-screen coordinates.
    /// </summary>
    public RECT rcMonitor;

    /// <summary>
    /// The work area rectangle of the display monitor, usable by applications, and expressed in virtual-screen coordinates.
    /// </summary>
    public RECT rcWork;

    /// <summary>
    /// A set of flags that represent attributes of the display monitor.
    /// </summary>
    public int dwFlags;

    /// <summary>
    /// A null-terminated string specifying the device name of the monitor.
    /// </summary>
    public fixed char szDevice[32];

    /// <summary>
    /// Gets a string specifying the device name of the monitor.
    /// </summary>
    public readonly ReadOnlySpan<char> Device
    {
        get
        {
            fixed (char* c = szDevice)
            {
                return new Span<char>(c, 32).SliceAtFirstNull();
            }
        }
    }

    /// <summary>
    /// Creates a <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions.
    /// </summary>
    /// <returns>A <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions.</returns>
    public static MONITORINFOEX CreateWritable()
        => new()
           {
               cbSize = (uint) sizeof(MONITORINFOEX)
           };
}