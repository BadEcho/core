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
    public async void Await_RunningAsync_WindowInitialized()
    {
        using var executor = new MessageOnlyExecutor();

        Assert.Null(executor.Window);

        await executor.RunAsync();

        Assert.NotNull(executor.Window);
    }

    [Fact]
    public void OperationStatus_RunningAsync_IsPending()
    {
        using var executor = new MessageOnlyExecutor();

        var operation = executor.RunAsync();

        Assert.Equal(ThreadExecutorOperationStatus.Pending, operation.Status);
    }

    [Fact]
    public void Run_Running_ThrowsException()
    {
        using var executor = CreateExecutor();

        while (executor.Window == null) { }

        Assert.Throws<InvalidOperationException>(executor.Run);
    }

    [Fact]
    public void RunAsync_Running_ThrowsException()
    {
        using var executor = CreateExecutor();

        Assert.ThrowsAsync<InvalidOperationException>(async () => await executor.RunAsync());
    }

    [Fact]
    public async void RunAsync_RequestsDisabled_ThrowsCatchableExecutorException()
    {
        using var executor = new MessageOnlyExecutor();
        bool caughtException = false;

        try
        {
            executor.Disable();

            await executor.RunAsync();
        }
        catch (InvalidOperationException)
        {   // This is thrown by the offloaded Run task. We need to see if it is still catchable in the current context.
            caughtException = true;
        }

        Assert.True(caughtException);
    }

    [Fact]
    public void Run_RunningThenDisposed_ThrowsException()
    {
        var executor = CreateExecutor();

        executor.Dispose();

        while (!executor.IsShutdownComplete) { }

        Assert.Throws<ObjectDisposedException>(executor.Run);
    }

    [Fact]
    public void RunAsync_RunningThenDisposed_ThrowsException()
    {
        var executor = CreateExecutor();

        executor.Dispose();

        while (!executor.IsShutdownComplete) { }

        Assert.ThrowsAsync<ObjectDisposedException>(async () => await executor.RunAsync());
    }

    [Fact]
    public void InvokeAction_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        executor.Invoke(() => Assert.Equal(executor.Thread.ManagedThreadId, Environment.CurrentManagedThreadId));
    }

    [Fact]
    public async void InvokeAction_RunningAsyncFromCallingThread_RunsOnExecutorThread()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.RunAsync();

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
    public async void InvokeAsync_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        var threadId = 
            await executor.InvokeAsync(() => Environment.CurrentManagedThreadId);

        Assert.Equal(executor.Thread.ManagedThreadId, threadId);
    }

    [Fact]
    public async void InvokeAsync_FromExecutorThread_RunsOnExecutorThread()
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
    public void InvokeAsync_ExceptionThrown_RaisedOnCallingThread()
    {
        using var executor = CreateExecutor();

        Assert.ThrowsAsync<BadImageFormatException>(
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
