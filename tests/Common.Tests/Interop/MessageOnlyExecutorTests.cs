// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;
using BadEcho.Threading;
using Xunit;

namespace BadEcho.Tests.Interop;

/// <suppressions>
/// ReSharper disable AccessToDisposedClosure
/// </suppressions>
public class MessageOnlyExecutorTests
{
    [Fact]
    public void Dispose_NotRunning_NoException()
    {
        using (var _ = new MessageOnlyExecutor())
        { }
    }

    [Fact]
    public void Dispose_Running_NoException()
    {
        var executor = CreateExecutor();

        executor.Dispose();
    }

    [Fact]
    public void InvokeDispose_Running_NoException()
    {
        var executor = CreateExecutor();

        executor.Invoke(executor.Dispose);
    }

    [Fact]
    public async Task Window_StartAsyncAwaited_WindowInitialized()
    {
        using var executor = new MessageOnlyExecutor();

        Assert.Null(executor.Window);

        await executor.StartAsync();

        Assert.NotNull(executor.Window);
    }

    [Fact]
    public void OperationStatus_StartAsyncNotAwaited_IsPending()
    {
        using var executor = new MessageOnlyExecutor();

        var operation = executor.StartAsync();

        Assert.Equal(ThreadExecutorOperationStatus.Pending, operation.Status);
    }

    [Fact]
    public void Run_AlreadyRunning_ThrowsException()
    {
        using var executor = CreateExecutor();

        while (executor.Window == null) { }

        Assert.Throws<InvalidOperationException>(executor.Run);
    }

    [Fact]
    public async Task StartAsync_AlreadyRunning_ThrowsException()
    {
        using var executor = new MessageOnlyExecutor();
        bool caughtException = false;

        await executor.StartAsync();

        try
        {
            await executor.StartAsync();
        }
        catch (InvalidOperationException)
        {
            caughtException = true;
        }

        Assert.True(caughtException);
    }

    [Fact]
    public async Task StartAsync_WithRequestsDisabled_ThrowsCatchableExecutorException()
    {
        using var executor = new MessageOnlyExecutor();
        bool caughtException = false;

        try
        {
            executor.Disable();

            await executor.StartAsync();
        }
        catch (InvalidOperationException)
        {   // This is thrown by the offloaded Run task. We need to see if it is still catchable in the current context.
            caughtException = true;
        }

        Assert.True(caughtException);
    }

    [Fact]
    public async Task Run_RunningThenDisposed_ThrowsException()
    {
        var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        executor.Dispose();

        while (!executor.IsShutdownComplete) { }

        Assert.Throws<ObjectDisposedException>(executor.Run);
    }

    [Fact]
    public void InvokeAction_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        executor.Invoke(() => Assert.Equal(executor.Thread.ManagedThreadId, Environment.CurrentManagedThreadId));
    }

    [Fact]
    public async Task InvokeAction_FromCallingThreadAfterStartAsync_RunsOnExecutorThread()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        int currentThreadId = 0;

        executor.Invoke(() => currentThreadId = Environment.CurrentManagedThreadId);

        Assert.Equal(executor.Thread.ManagedThreadId, currentThreadId);
    }

    [Fact]
    public void InvokeFunc_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        var localThreadId = executor.Invoke(() => Environment.CurrentManagedThreadId);

        Assert.Equal(executor.Thread.ManagedThreadId, localThreadId);
    }

    [Fact]
    public void InvokeAction_FromExecutorThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        executor.Invoke(() => executor.Invoke(() => Assert.Equal(executor.Thread.ManagedThreadId,
                                                                 Environment.CurrentManagedThreadId)));
    }

    [Fact]
    public void InvokeFunc_FromExecutorThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        var localThreadId = executor.Invoke(() => executor.Invoke(() => Environment.CurrentManagedThreadId));

        Assert.Equal(executor.Thread.ManagedThreadId, localThreadId);
    }

    [Fact]
    public async Task InvokeAsync_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        var threadId = 
            await executor.InvokeAsync(() => Environment.CurrentManagedThreadId);

        Assert.Equal(executor.Thread.ManagedThreadId, threadId);
    }

    [Fact]
    public async Task InvokeAsync_FromExecutorThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        var localThreadId =
            await executor.InvokeAsync(async () =>
                                       {
                                           var threadId =
                                               await executor.InvokeAsync(() => Environment.CurrentManagedThreadId);

                                           return threadId;
                                       });

        Assert.Equal(executor.Thread.ManagedThreadId, localThreadId?.Result);
    }

    [Fact]
    public void Invoke_ExceptionThrown_RaisedOnCallingThread()
    {
        using var executor = CreateExecutor();

        Assert.Throws<BadImageFormatException>(() => executor.Invoke(() => throw new BadImageFormatException("Holy cow!")));
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_RaisedOnCallingThread()
    {
        using var executor = CreateExecutor();

        await Assert.ThrowsAsync<BadImageFormatException>(
            async () => await executor.InvokeAsync(() => throw new BadImageFormatException("Holy cow!")));
    }

    [Fact]
    public void WaitOnOperation_CallingThreadNoTimeout_OperationEndsFirst()
    {
        using var executor = CreateExecutor();

        var operation = executor.InvokeAsync(() => Thread.Sleep(2000));

        operation.Wait(TimeSpan.FromSeconds(10));
        Assert.Equal(ThreadExecutorOperationStatus.Completed, operation.Status);
    }

    [Fact]
    public void WaitOnOperation_CallingThreadTimeout_TimesOutFirst()
    {
        using var executor = CreateExecutor();

        var operation = executor.InvokeAsync(() => Thread.Sleep(5000));
        
        operation.Wait(TimeSpan.FromSeconds(1));

        Assert.Equal(ThreadExecutorOperationStatus.Running, operation.Status);
    }

    [Fact]
    public void WaitOnOperation_ExecutorThreadNoTimeout_ResumesExecution()
    {
        bool executionResumed = false;
        using var executor = CreateExecutor();

        executor.Invoke(() =>
                        {
                            var operation = executor.InvokeAsync(() => Thread.Sleep(2000));

                            operation.Wait(TimeSpan.FromSeconds(10));
                            executionResumed = true;
                        });
        // The operation will always complete before a wait operation executing on the executor
        // thread returns (no way around this), so we just check if execution resumed normally.
        Assert.True(executionResumed);
    }

    [Fact]
    public void WaitOnOperation_ExecutorThreadTimeout_ResumesExecution()
    {
        bool executionResumed = false;
        using var executor = CreateExecutor();

        executor.Invoke(() =>
                        {
                            var operation = executor.InvokeAsync(() => Thread.Sleep(5000));

                            operation.Wait(TimeSpan.FromSeconds(1));
                            executionResumed = true;
                        });
        // The operation will always complete before a wait operation executing on the executor
        // thread returns (no way around this), so we just check if execution resumed normally.
        Assert.True(executionResumed);
    }

    private static MessageOnlyExecutor CreateExecutor()
    {
        var executor = new MessageOnlyExecutor();

        Task.Run(executor.Run);

        return executor;
    }
}
