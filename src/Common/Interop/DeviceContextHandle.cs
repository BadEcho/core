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
/// Provides a level-0 type for device context handles.
/// </summary>
/// <remarks>
/// Device contexts are opaque data structures used internally by GDI, primarily for the purposes related to graphic and drawing.
/// </remarks>
internal sealed class DeviceContextHandle : SafeHandle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceContextHandle"/> class.
    /// </summary>
    private DeviceContextHandle()
        : base(IntPtr.Zero, true)
    { }

    /// <summary>
    /// Gets a null device context handle.
    /// </summary>
    public static DeviceContextHandle Null
        => new();

    /// <inheritdoc/>
    public override bool IsInvalid
        => handle == IntPtr.Zero;

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => User32.ReleaseDC(IntPtr.Zero, handle) == 1;
}