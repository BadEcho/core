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

namespace BadEcho.Interop;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided in-process window and the messages it receives.
/// </summary>
/// <remarks>
/// Because this wrapper intercepts window messages by subclassing the provided window, the window being wrapped must be 
/// local to the same process hosting the .NET runtime that is executing this code. If the window was created in another 
/// process, you'll need to make use of the <c>BadEcho.Hooks</c> library instead.
/// </remarks>
/// <suppressions>
/// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
/// </suppressions>
public sealed class LocalWindowWrapper : WindowWrapper, IDisposable
{
    private readonly WindowProcedure _callback;
    private readonly WindowSubclass _subclass;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalWindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle to the window being wrapped.</param>
    public LocalWindowWrapper(WindowHandle handle) 
        : base(handle)
    {   // We need to pin this delegate, since the subclass only weakly references it.
        _callback = WindowProcedure;
        _subclass = new WindowSubclass(_callback);

        _subclass.Attach(handle);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _subclass.Dispose();

        _disposed = true;
    }
}