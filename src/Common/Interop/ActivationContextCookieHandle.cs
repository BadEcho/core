// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Win32.SafeHandles;

namespace BadEcho.Interop;

/// <summary>
/// Provides a level-0 type for activation context cookies, which are generated when activating
/// contexts.
/// </summary>
internal sealed class ActivationContextCookieHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly int _owningThreadId = Environment.CurrentManagedThreadId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivationContextCookieHandle"/> class.
    /// </summary>
    public ActivationContextCookieHandle()
        : base(true) 
    { }

    /// <summary>
    /// Gets a value indicating if this handle was created on the current thread.
    /// </summary>
    public bool IsOwnedByCurrentThread
        => Environment.CurrentManagedThreadId == _owningThreadId;

    /// <inheritdoc/>
    /// <remarks>Context deactivation has to occur on the same thread that activated it.</remarks>
    protected override bool ReleaseHandle()
        => IsOwnedByCurrentThread && Kernel32.DeactivateActCtx(0, handle);
}
