//-----------------------------------------------------------------------
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
    public DeviceContextHandle()
        : this(true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceContextHandle"/> class.
    /// </summary>
    /// <param name="ownsHandle">
    /// Value indicating if this safe handle is responsible for releasing the provided handle.
    /// </param>
    private DeviceContextHandle(bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
    { }

    /// <summary>
    /// Gets a default invalid instance of the <see cref="DeviceContextHandle"/> class.
    /// </summary>
    public static DeviceContextHandle InvalidHandle
        => new(false);

    /// <inheritdoc/>
    public override bool IsInvalid
        => handle == IntPtr.Zero;

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => User32.ReleaseDC(IntPtr.Zero, handle) == 1;
}