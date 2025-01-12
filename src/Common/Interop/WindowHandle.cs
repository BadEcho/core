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

using System.ComponentModel;
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
        : this(true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    /// <param name="ownsHandle">Value indicating if this safe handle is responsible for releasing the provided handle.</param>
    private WindowHandle(bool ownsHandle)
        : base(IntPtr.Zero, ownsHandle)
    { }

    /// <summary>
    /// Gets a default invalid instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    public static WindowHandle InvalidHandle
        => new(false);

    /// <inheritdoc/>
    public override bool IsInvalid
        => handle == IntPtr.Zero;

    /// <summary>
    /// Retrieves the identifier of the thread that created the window.
    /// </summary>
    /// <returns>The identifier of the thread that created the window.</returns>
    /// <exception cref="Win32Exception">The unmanaged function used to retrieve the thread identifier failed.</exception>
    public int GetThreadId()
    {
        var threadId = (int) User32.GetWindowThreadProcessId(this, IntPtr.Zero);
        
        if (threadId == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return threadId;
    }
    
    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => User32.DestroyWindow(handle);
}