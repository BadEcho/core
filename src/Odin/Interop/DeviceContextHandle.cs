//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
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