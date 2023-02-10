//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
    public WindowHandle(IntPtr handle, bool ownsHandle)
        : this(ownsHandle)
    {
        SetHandle(handle);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    public WindowHandle()
        : base(IntPtr.Zero, true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    /// <param name="ownsHandle">Value indicating if this safe handle is responsible for releasing the provided handle.</param>
    private WindowHandle(bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
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