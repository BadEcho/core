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
    public void Initialize_Dispose()
    {
        using (var _ = new MessageOnlyExecutor())
        { }
    }

    [Fact]
    public void InvokeAction_FromCallingThread_RunsOnExecutorThread()
    {
        using var executor = CreateExecutor();

        executor.Invoke(() => Assert.Equal(executor.Thread.ManagedThreadId, Environment.CurrentManagedThreadId));
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
        var mre = new ManualResetEvent(false);

        MessageOnlyExecutor? executor = null;

        Task.Run(() =>
                 {
                     executor = new MessageOnlyExecutor();
                     mre.Set();
                     executor.Run();
                 });

        mre.WaitOne();

        Assert.NotNull(executor);

        return executor;
    }
}
