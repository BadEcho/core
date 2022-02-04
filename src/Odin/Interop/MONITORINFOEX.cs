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

using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop;

/// <summary>
/// Provides information about a display monitor.
/// </summary>
/// <remarks>
/// In order to create a <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions, one must make
/// use of the <see cref="CreateWritable"/> method.
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
internal struct MONITORINFOEX
{
    /// <summary>
    /// The size, in bytes, of the structure.
    /// </summary>
    public int cbSize;

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
    /// The device name of the monitor.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public char[] szDevice;

    /// <summary>
    /// Creates a <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions.
    /// </summary>
    /// <returns>A <see cref="MONITORINFOEX"/> value that can be written to by unmanaged functions.</returns>
    public static MONITORINFOEX CreateWritable()
        => new()
           {
               cbSize = Marshal.SizeOf(typeof(MONITORINFOEX)),
               rcMonitor = new RECT(),
               rcWork = new RECT(),
               dwFlags = 0,
               szDevice = new char[32]
           };
}