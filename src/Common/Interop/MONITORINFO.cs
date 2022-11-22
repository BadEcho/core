//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides information about a display monitor.
/// </summary>
/// <remarks>
/// In order to create a <see cref="MONITORINFO"/> value that can be written to by unmanaged functions, one must make
/// use of the <see cref="CreateWritable"/> method.
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
internal struct MONITORINFO
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
    /// Creates a <see cref="MONITORINFO"/> value that can be written to by unmanaged functions.
    /// </summary>
    /// <returns>A <see cref="MONITORINFO"/> value that can be written to by unmanaged functions.</returns>
    public static MONITORINFO CreateWritable()
        => new()
           {
               cbSize = Marshal.SizeOf(typeof(MONITORINFO)),
               rcMonitor = new RECT(),
               rcWork = new RECT(),
               dwFlags = 0
           };
}