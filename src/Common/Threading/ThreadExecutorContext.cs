//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Interop;

namespace BadEcho.Threading;

/// <summary>
/// Provides a synchronization context for a thread executor.
/// </summary>
internal sealed class ThreadExecutorContext : SynchronizationContext
{
    private readonly IThreadExecutor _executor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorContext"/> class.
    /// </summary>
    /// <param name="executor">The executor that the context is for.</param>
    public ThreadExecutorContext(IThreadExecutor executor)
    {
        Require.NotNull(executor, nameof(executor));

        _executor = executor;

        SetWaitNotificationRequired();
    }

    /// <inheritdoc/>
    public override SynchronizationContext CreateCopy()
    {
        return new ThreadExecutorContext(_executor);
    }

    /// <inheritdoc/>
    public override void Send(SendOrPostCallback d, object? state)
    {
        _executor.Invoke(d, true, state);
    }

    /// <inheritdoc/>
    public override void Post(SendOrPostCallback d, object? state)
    {
        _executor.BeginInvoke(d, true, state);
    }

    /// <inheritdoc/>
    public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
    {
        if (_executor.DisableRequests > 0)
        {
            return Kernel32.WaitForMultipleObjectsEx(waitHandles.Length,
                                                     waitHandles,
                                                     waitAll,
                                                     (uint) millisecondsTimeout,
                                                     false);
        }

        return WaitHelper(waitHandles, waitAll, millisecondsTimeout);
    }
}
