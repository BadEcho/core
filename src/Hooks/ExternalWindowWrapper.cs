// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

using BadEcho.Interop;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a wrapper around an <c>HWND</c> of a provided out-of-process window and the messages it receives.
/// </summary>
/// <remarks>
/// <para>
/// Window messages intended for other applications cannot be intercepted via the usual technique of subclassing, as it is not
/// possible to subclass a window created by and running on another process.
/// </para>
/// <para>
/// Instead, a hook procedure needs to be installed and associated with the thread owning the wrapped window.
/// See https://badecho.com/index.php/2024/01/13/external-window-messages/ for more information on the topic.
/// </para>
/// </remarks>
public sealed class ExternalWindowWrapper : WindowWrapper, IDisposable
{
    private readonly WindowSource _source;

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalWindowWrapper"/> class.
    /// </summary>
    /// <param name="handle">A handle to the window being wrapped.</param>
    public ExternalWindowWrapper(WindowHandle handle)
        : base(handle)
    {
        int threadId = Handle.GetThreadId();

        _source = new WindowSource(threadId);
        _source.AddCallback(WindowProcedure);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;
        
        _source.Dispose();

        _disposed = true;
    }
    
    /// <inheritdoc/>
    protected override void OnDestroyingWindow()
    {
        base.OnDestroyingWindow();

        Dispose();
    }
}