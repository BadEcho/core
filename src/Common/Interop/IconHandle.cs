//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Provides a level-0 type for icon handles.
/// </summary>
internal sealed class IconHandle : SafeHandle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IconHandle"/> class.
    /// </summary>
    public IconHandle()
        : base(IntPtr.Zero, true)
    { }

    /// <inheritdoc/>
    public override bool IsInvalid
        => handle == IntPtr.Zero;

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => User32.DestroyIcon(handle);
}
