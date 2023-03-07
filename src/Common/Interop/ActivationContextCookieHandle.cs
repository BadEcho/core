//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Win32.SafeHandles;

namespace BadEcho.Interop;

/// <summary>
/// Provides a level-0 type for activation context cookies, which are generated when activating
/// contexts.
/// </summary>
internal sealed class ActivationContextCookieHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivationContextCookieHandle"/> class.
    /// </summary>
    internal ActivationContextCookieHandle()
        : base(true) 
    { }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
        => Kernel32.DeactivateActCtx(0, handle);
}
