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

using Microsoft.Win32.SafeHandles;

namespace BadEcho.Interop;

/// <summary>
/// Provides a level-0 type for activation context handles.
/// </summary>
internal sealed class ActivationContextHandle : SafeHandleMinusOneIsInvalid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivationContextHandle"/> class.
    /// </summary>
    public ActivationContextHandle() 
        : base(true)
    { }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {   
        Kernel32.ReleaseActCtx(handle);

        return true;
    }
}
