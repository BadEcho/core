//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Properties;
using BadEcho.Odin.Threading;
using ThreadExceptionEventArgs = BadEcho.Odin.Threading.ThreadExceptionEventArgs;

namespace BadEcho.Odin.Interop;

/// <summary>
/// Provides an entity responsible for the execution of an action through a particular thread or context through the use of a
/// message-only window.
/// </summary>
public sealed class MessageOnlyExecutor : IThreadExecutor, IDisposable
{
    private static readonly WindowMessage _ProcessOperation
        = User32.RegisterWindowMessage("MessageOnlyExecutor.ProcessOperation");
    
    private static readonly List<WeakReference<IThreadExecutor>> _Executors 
        = new();

    private static readonly object _ExceptionProcessedKey 
        = new();

    private static readonly object _ExecutorsLock
        = new();

    private static WeakReference<IThreadExecutor>? _LastExecutor;

    private readonly List<ThreadExecutorOperation> _operationQueue
        = new();

    private readonly ThreadExecutorInvokeFilter _invokeFilter;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // We need to keep a reference to this so it stays alive, as the window wrapper it is provided to stores it in a weak list.
    private readonly WindowProc _hook;

    private ExecutionContext? _shutdownContext;
    private int _framesRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageOnlyExecutor"/> class.
    /// </summary>
    public MessageOnlyExecutor()
    {
        lock (_ExecutorsLock)
        {
            _Executors.Add(new WeakReference<IThreadExecutor>(this));
        }

        _invokeFilter = new ThreadExecutorInvokeFilter(this);
        _invokeFilter.Filter += HandleExceptionFilter;

        Thread = Thread.CurrentThread;
        Window = new MessageOnlyWindowWrapper(this);

        _hook = WndProc;
        Window.AddStartingHook(_hook);
    }

    /// <inheritdoc/>
    public event EventHandler<ThreadExceptionEventArgs>? UnhandledException;

    /// <inheritdoc/>
    public bool IsShutdownStarted 
    { get; private set; }

    /// <inheritdoc/>
    public bool IsShutdownComplete 
    { get; private set; }

    /// <inheritdoc/>
    public int DisableRequests 
    { get; private set; }

    /// <inheritdoc/>
    public object Lock 
    { get; } = new();

    /// <inheritdoc/>
    public Thread Thread 
    { get; }

    /// <summary>
    /// Gets a wrapper around the message-only window being used by this executor to send and receive messages.
    /// </summary>
    public MessageOnlyWindowWrapper? Window
    { get; private set; }

    object? IThreadExecutor.Invoke(Delegate method, bool filterExceptions, object? argument)
        => DirectlyInvoke(method, filterExceptions, argument);

    void IThreadExecutor.BeginInvoke(Delegate method, bool filterExceptions, object? argument)
    {
        var operation 
            = new ThreadExecutorOperation(this, method, filterExceptions, argument);

        InvokeAsync(operation);
    }

    /// <summary>
    /// Retrieves the executor created on the specified thread, if one exists.
    /// </summary>
    /// <param name="thread">The thread on which an executor was created.</param>
    /// <returns>
    /// The <see cref="IThreadExecutor"/> instance created on <c>thread</c>; or null, if none was created on <c>thread</c>.
    /// </returns>
    public static IThreadExecutor? CreatedOn(Thread thread)
    {
        lock (_ExecutorsLock)
        {   // We have the last executor accessed cached for quick returning, otherwise we maintain a collection of other executors
            // we go through, and we'll return any entry we find created on the provided thread.
            if (_LastExecutor == null || !_LastExecutor.TryGetTarget(out IThreadExecutor? target) || target.Thread != thread)
            {
                target = null;

                for (int i = 0; i < _Executors.Count; i++)
                {
                    if (_Executors[i].TryGetTarget(out IThreadExecutor? executor) && executor.Thread == thread)
                    {
                        target = executor;
                    }
                    else
                    {
                        _Executors.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (target != null)
            {
                if (_LastExecutor == null)
                    _LastExecutor = new WeakReference<IThreadExecutor>(target);
                else
                    _LastExecutor.SetTarget(target);
            }

            return target;
        }
    }

    /// <inheritdoc/>
    public void Invoke(Action method)
    {
        Require.NotNull(method, nameof(method));

        DirectlyInvoke(method, false, null);
    }

    /// <inheritdoc/>
    public TResult? Invoke<TResult>(Func<TResult> method)
    {
        Require.NotNull(method, nameof(method));

        return (TResult?) DirectlyInvoke(method, false, null);
    }

    /// <inheritdoc/>
    public ThreadExecutorOperation InvokeAsync(Action method)
    {
        Require.NotNull(method, nameof(method));

        var operation
            = new ThreadExecutorOperation(this, method, false, null);

        InvokeAsync(operation);

        return operation;
    }

    /// <inheritdoc/>
    public ThreadExecutorOperation<TResult?> InvokeAsync<TResult>(Func<TResult> method)
    {
        Require.NotNull(method, nameof(method));

        var operation
            = new ThreadExecutorOperation<TResult?>(this, method, false);

        InvokeAsync(operation);

        return operation;
    }

    /// <inheritdoc/>
    public bool Cancel(ThreadExecutorOperation operation)
    {
        Require.NotNull(operation, nameof(operation));

        lock (Lock)
        {
            if (!_operationQueue.Contains(operation))
                return false;

            _operationQueue.Remove(operation);
        }

        operation.Status = ThreadExecutorOperationStatus.Canceled;

        return true;
    }

    /// <inheritdoc/>
    public void Disable()
    {
        if (Thread != Thread.CurrentThread)
            throw new InvalidOperationException(Strings.ExecutorCannotDisableOnOtherThread);

        DisableRequests++;
    }

    /// <inheritdoc/>
    public void Enable()
    {
        if (DisableRequests == 0)
            return;

        if (Thread != Thread.CurrentThread)
            throw new InvalidOperationException(Strings.ExecutorCannotEnableOnOtherThread);

        DisableRequests--;
    }

    /// <inheritdoc/>
    public IThreadExecutorFrame CreateFrame()
        => new MessageOnlyExecutorFrame(this);

    /// <inheritdoc/>
    public void PushFrame(IThreadExecutorFrame frame)
    {
        Require.NotNull(frame, nameof(frame));

        if (IsShutdownComplete)
            throw new InvalidOperationException(Strings.ExecutorShutdown);

        if (DisableRequests > 0)
            throw new InvalidOperationException(Strings.ExecutorProcessingDisabled);

        _framesRunning++;

        try
        {
            SynchronizationContext? oldContext = SynchronizationContext.Current;
            var newContext = new ThreadExecutorContext(this);

            SynchronizationContext.SetSynchronizationContext(newContext);

            try
            {
                LoopMessages(frame);

                if (_framesRunning == 1 && IsShutdownStarted)
                    Shutdown();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }
        finally
        {
            _framesRunning--;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Invoke(StartShutdown);

        _shutdownContext?.Dispose();
    }

    private static void LoopMessages(IThreadExecutorFrame frame)
    {
        var msg = new MSG();

        while (frame.ShouldContinue)
        {   // This is very much the style of the traditional Win32 message queue loop. Get, Translate, and then Dispatch the message.
            if (!User32.GetMessage(ref msg, IntPtr.Zero, 0, 0))
                break;
            
            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }
    }
    
    private static void HandleExceptionFilter(object? sender, ThreadExceptionEventArgs e)
    {
        if (sender == null)
            return;

        var executor = (MessageOnlyExecutor)sender;

        e.Handled = executor.FilterException(e.Data);
    }

    private void InvokeAsync(ThreadExecutorOperation operation)
    {
        bool succeeded = false;

        lock (Lock)
        {
            if (!Environment.HasShutdownStarted && !IsShutdownComplete)
            {
                _operationQueue.Insert(0, operation);

                succeeded = RequestProcessOperation();

                if (!succeeded)
                    _operationQueue.Remove(operation);
            }
        }

        if (!succeeded)
        {   // We mark the operation as canceled since we failed to enqueue it -- we just set the appropriate statuses here as opposed
            // to calling cancel as the operation is already removed from the queue.
            operation.Status = ThreadExecutorOperationStatus.Canceled;
            operation.TaskSource.SetCanceled();
        }
    }

    private object? DirectlyInvoke(Delegate method, bool filterExceptions, object? argument)
    {
        if (Thread == Thread.CurrentThread)
        {
            SynchronizationContext? oldContext = SynchronizationContext.Current;

            try
            {
                var newContext = new ThreadExecutorContext(this);

                SynchronizationContext.SetSynchronizationContext(newContext);

                return _invokeFilter.Execute(method, filterExceptions, argument);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        var operation
            = new ThreadExecutorOperation(this, method, filterExceptions, argument);

        InvokeAsync(operation);
        operation.Wait();

        return operation.Result;
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var message = (WindowMessage) msg;

        if (DisableRequests > 0)
            throw new InvalidOperationException(Strings.ExecutorDisabledByQueuePumping);

        if (WindowMessage.Destroy == message)
        {
            if (!IsShutdownStarted && !IsShutdownComplete)
                Shutdown();
        } 
        else if (_ProcessOperation == message)
        {
            ProcessOperation();
        }

        return IntPtr.Zero;
    }

    private void ProcessOperation()
    {
        ThreadExecutorOperation? operation = null;

        lock (Lock)
        {
            if (_operationQueue.Count > 0)
                operation = Dequeue();

            RequestProcessOperation();
        }

        if (operation == null)
            return;

        operation.Invoke();
        operation.FinalizeCompletion();
    }

    private bool RequestProcessOperation()
    {
        if (Window == null || _operationQueue.Count == 0)
            return false;

        return User32.PostMessage(Window.Handle, _ProcessOperation, IntPtr.Zero, IntPtr.Zero);
    }

    /// <remarks>
    /// This should always be called within a lock on <see cref="Lock"/>.
    /// </remarks>
    private ThreadExecutorOperation? Dequeue()
    {
        if (_operationQueue.Count == 0)
            return null;

        ThreadExecutorOperation operation = _operationQueue[^1];

        _operationQueue.Remove(operation);

        return operation;
    }

    private bool FilterException(Exception exception)
    {   // There's a chance this is hit multiple times for the same exception, especially if we get reentered after the fact.
        // So we just make sure this is our first time seeing.
        if (exception.Data.Contains(_ExceptionProcessedKey))
            return false;

        exception.Data.Add(_ExceptionProcessedKey, null);

        EventHandler<ThreadExceptionEventArgs>? unhandledException = UnhandledException;

        if (unhandledException == null)
            return false;

        var eventArgs = new ThreadExceptionEventArgs(exception);

        unhandledException(this, eventArgs);

        return eventArgs.Handled;
    }

    private void StartShutdown()
    {
        if (IsShutdownStarted)
            return;

        IsShutdownStarted = true;

        _shutdownContext = ExecutionContext.Capture();

        if (_framesRunning == 0)
            Shutdown();
    }

    private void Shutdown()
    {
        if (IsShutdownComplete)
            return;

        if (_shutdownContext != null)
            ExecutionContext.Run(_shutdownContext, ShutdownContextCallback, null);
        else
            ShutdownContextCallback(null);

        
        _shutdownContext = null;
    }

    private void ShutdownContextCallback(object? state)
    {
        MessageOnlyWindowWrapper? window;

        lock (Lock)
        {
            window = Window;

            Window = null;
        }

        window?.Dispose();

        lock (Lock)
        {
            IsShutdownComplete = true;
        }

        ThreadExecutorOperation? operation;

        do
        {
            lock (Lock)
            {
                operation = Dequeue();
            }

            // We must lock when reading from the queue, however we don't want to lock while the operation's cancellation
            // routines run.
            operation?.Cancel();

        } while (operation != null);
    }
}
