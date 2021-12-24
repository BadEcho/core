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
/// Provides a level-0 type for window handles.
/// </summary>
/// <suppressions>
/// ReSharper disable UnusedMember.Local
/// </suppressions>
public sealed class WindowHandle : SafeHandle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    /// <param name="handle">The handle to the window.</param>
    /// <param name="ownsHandle">Value indicating if this safe handle is responsible for releasing the provided handle.</param>
    internal WindowHandle(IntPtr handle, bool ownsHandle)
        : this(ownsHandle)
    {
        SetHandle(handle);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    /// <param name="ownsHandle">Value indicating if this safe handle is responsible for releasing the provided handle.</param>
    private WindowHandle(bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    private WindowHandle()
        : base (IntPtr.Zero, true)
    { }

    /// <inheritdoc/>
    public override bool IsInvalid
        => handle == IntPtr.Zero;

    /// <summary>
    /// Gets a default invalid instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    internal static WindowHandle InvalidHandle 
        => new(IntPtr.Zero, false);

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => User32.DestroyWindow(handle);
}