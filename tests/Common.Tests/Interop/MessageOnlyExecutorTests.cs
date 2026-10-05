// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
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

[CollectionDefinition("MessageOnlyExecutor", DisableParallelization = true)]
public sealed class MessageOnlyExecutorTestsDefinition;

/// <suppressions>
/// ReSharper disable AccessToDisposedClosure
/// </suppressions>
[Collection("MessageOnlyExecutor")]
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
    public async Task Dispose_WithPendingCrossThreadOperation_CancelsOperation()
    {
        var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        using var mre = new ManualResetEventSlim(false);

        _ = executor.InvokeAsync(() =>
        {
            mre.Wait();
            executor.Dispose();
        });

        
        var pending = executor.InvokeAsync(() => 42);

        mre.Set();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending.Task.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Equal(ThreadExecutorOperationStatus.Canceled, pending.Status);
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

    [SkipOnGitHubFact]
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
    public async Task Dispose_RacingStartup_ShutsDown()
    {
        for (int i = 0; i < 100; i++)
        {
            var executor = new MessageOnlyExecutor();
            Task run = Task.Run(executor.Run);

            // The window is assigned while Run() holds the executor's lock, just before it releases it to push its frame.
            Assert.True(SpinWait.SpinUntil(() => executor.Window != null || run.IsCompleted, TimeSpan.FromSeconds(5)));

            executor.Dispose();

            // Run() only returns once the outermost frame has exited and shutdown has completed.
            await run.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.True(executor.IsShutdownComplete);
        }
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
    public async Task Cancel_WhileOtherThreadWaits_ReleasesWaiter()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        for (int i = 0; i < 100; i++)
        {
            using var blocker = new ManualResetEventSlim(false);
            // Occupies the executor so the next operation stays queued.
            ThreadExecutorOperation blocking = executor.InvokeAsync(() => blocker.Wait());
            ThreadExecutorOperation pending = executor.InvokeAsync(() => { });

            Task<ThreadExecutorOperationStatus> waiter = Task.Run(() =>
            {
                try
                {
                    pending.Wait();
                }
                catch (OperationCanceledException)
                {   // Wait surfaces cancellation of known delegate types through the operation's task.
                }

                return pending.Status;
            });

            try
            {
                Assert.True(pending.Cancel());

                ThreadExecutorOperationStatus status = await waiter.WaitAsync(TimeSpan.FromSeconds(5));

                Assert.Equal(ThreadExecutorOperationStatus.Canceled, status);
            }
            finally
            {
                blocker.Set();
                blocking.Wait();
            }
        }
    }

    [Fact]
    public async Task Cancel_PendingOperation_RaisesCanceledOnce()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        using var blocker = new ManualResetEventSlim(false);
        ThreadExecutorOperation blocking = executor.InvokeAsync(() => blocker.Wait());
        ThreadExecutorOperation pending = executor.InvokeAsync(() => { });
        int raised = 0;

        pending.Canceled += (_, _) => Interlocked.Increment(ref raised);

        try
        {
            Assert.True(pending.Cancel());
            Assert.Equal(ThreadExecutorOperationStatus.Canceled, pending.Status);
            Assert.Equal(1, raised);
        }
        finally
        {
            blocker.Set();
            blocking.Wait();
        }
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

        operation.Wait();
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

    [SkipOnGitHubFact]
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

    [Fact]
    public async Task SynchronizationContextSend_FromOtherThread_RunsOnExecutorThread()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        var context = executor.Invoke(() => SynchronizationContext.Current);
        Assert.NotNull(context);

        int sendThreadId = 0;

        Task sendTask = Task.Run(() => context.Send(_ => sendThreadId = Environment.CurrentManagedThreadId, null));

        await sendTask.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(executor.Thread.ManagedThreadId, sendThreadId);
    }

    [Fact]
    public async Task Post_ThrowingCallback_RaisesUnhandledExceptionAndKeepsRunning()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        var reported = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);

        executor.UnhandledException += (_, e) =>
        {
            e.Handled = true;
            reported.TrySetResult(e.Data);
        };

        SynchronizationContext? context = executor.Invoke(() => SynchronizationContext.Current);

        Assert.NotNull(context);

        context.Post(_ => throw new InvalidOperationException(), null);

        Exception exception = await reported.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.IsType<InvalidOperationException>(exception);

        Assert.Equal(executor.Thread.ManagedThreadId, executor.Invoke(() => Environment.CurrentManagedThreadId));
    }

    [Fact]
    public async Task Run_QuitMessageEndsLoop_ShutsDownAndDestroysWindow()
    {
        using var executor = new MessageOnlyExecutor();
        Task run = Task.Run(executor.Run);

        // Completes once the executor is running.
        await executor.InvokeAsync(() => { }).Task.WaitAsync(TimeSpan.FromSeconds(5));

        MessageOnlyWindowWrapper? window = executor.Window;

        Assert.NotNull(window);

        IntPtr hWnd = window.Handle.DangerousGetHandle();
        const WindowMessage quit = (WindowMessage)0x12; // WM_QUIT

        Assert.True(User32.PostMessage(window.Handle, quit, IntPtr.Zero, IntPtr.Zero));

        await run.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.True(executor.IsShutdownComplete);
        Assert.Equal(0u, User32.GetWindowThreadProcessId(hWnd, out _));
    }

    [Fact]
    public async Task StartAsync_WithRequestsDisabled_CreatesNoWindow()
    {
        using var executor = new MessageOnlyExecutor();

        executor.Disable();

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await executor.StartAsync());

        Assert.Null(executor.Window);
    }
    private static MessageOnlyExecutor CreateExecutor()
    {
        var executor = new MessageOnlyExecutor();

        Task.Run(executor.Run);

        return executor;
    }
}
