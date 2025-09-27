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

using BadEcho.Properties;
using BadEcho.Threading;

namespace BadEcho.Interop;

/// <summary>
/// Provides an entity responsible for the execution of an action through a particular thread or context through the use of a
/// message-only window.
/// </summary>
public sealed class MessageOnlyExecutor : IThreadExecutor, IDisposable
{
    private static readonly WindowMessage _ProcessOperation
        = User32.RegisterWindowMessage("MessageOnlyExecutor.ProcessOperation");
    
    private static readonly List<WeakReference<IThreadExecutor>> _Executors = [];
    private static readonly Lock _ExecutorsLock = new();

    private static WeakReference<IThreadExecutor>? _LastExecutor;

    private readonly List<ThreadExecutorOperation> _operationQueue = [];
    private readonly ManualResetEventSlim _running = new();
    
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // The window wrapper this is provided to will store it in a weak list,
    // so we need to keep a reference to it to keep it alive.
    private readonly WindowProcedure _callback;
    private readonly WeakReference<IThreadExecutor> _thisExecutor;

    private ExecutionContext? _shutdownContext;
    private int _framesRunning;
    private bool _hasStarted;
    private bool _disposeQueued;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageOnlyExecutor"/> class.
    /// </summary>
    public MessageOnlyExecutor()
    {
        lock (_ExecutorsLock)
        {
            _thisExecutor = new WeakReference<IThreadExecutor>(this);
            _Executors.Add(_thisExecutor);
        }

        Thread = Thread.CurrentThread;
        _callback = WindowProcedure;
    }

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
    { get; private set; }

    /// <summary>
    /// Gets a wrapper around the message-only window being used by this executor to send and receive messages.
    /// </summary>
    public MessageOnlyWindowWrapper? Window
    { get; private set; }

    object? IThreadExecutor.Invoke(Delegate method, object? argument)
    {
        if (Thread == Thread.CurrentThread) 
        {
            return InvokeContext(ExecuteDelegate);

            object? ExecuteDelegate()
            {
                switch (method)
                {
                    case Action action:
                        action();
                        return null;

                    case Func<object> function:
                        return function();

                    case Func<object?,object> function:
                        return function(argument);

                    case SendOrPostCallback callback:
                        callback(argument);
                        return null;

                    default:
                        return method.DynamicInvoke(argument);
                }
            }
        }

        var operation = new ThreadExecutorOperation(this, method, true, argument);
        operation.Wait();

        return operation.Result;
    }

    void IThreadExecutor.BeginInvoke(Delegate method, object? argument)
    {
        var operation 
            = new ThreadExecutorOperation(this, method, true, argument);

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

        if (OnExecutorThread())
        {
            InvokeContext(ExecuteAction);

            object? ExecuteAction()
            {
                method();

                return null;
            }
        }
        else
        {
            var operation = new ThreadExecutorOperation(this, method);
            
            InvokeAsync(operation);
            operation.Wait();
        }
    }

    /// <inheritdoc/>
    public TResult? Invoke<TResult>(Func<TResult> method)
    {
        Require.NotNull(method, nameof(method));

        if (OnExecutorThread())
            return (TResult?) InvokeContext(() => method());

        var operation
            = new ThreadExecutorOperation<TResult>(this, method);

        InvokeAsync(operation);
        operation.Wait();

        return operation.Result;
    }

    /// <inheritdoc/>
    public ThreadExecutorOperation InvokeAsync(Action method)
    {
        Require.NotNull(method, nameof(method));

        var operation
            = new ThreadExecutorOperation(this, method);

        InvokeAsync(operation);

        return operation;
    }

    /// <inheritdoc/>
    public ThreadExecutorOperation<TResult?> InvokeAsync<TResult>(Func<TResult> method)
    {
        Require.NotNull(method, nameof(method));

        var operation = new ThreadExecutorOperation<TResult?>(this, method);

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
        => CreateFrame(false);

    /// <inheritdoc/>
    public void PushFrame(IThreadExecutorFrame frame)
    {
        Require.NotNull(frame, nameof(frame));

        if (IsShutdownComplete)
            throw new InvalidOperationException(Strings.ExecutorShutdown);

        if (DisableRequests > 0)
            throw new InvalidOperationException(Strings.ExecutorProcessingDisabled);

        if (Window == null)
            throw new InvalidOperationException(Strings.ExecutorFramesRequireRun);

        _framesRunning++;
        _hasStarted = true;

        try
        {
            SynchronizationContext? oldContext = SwitchContext();

            try
            {
                LoopMessages(frame);
                // If this is the last frame to exit following a shutdown request, then it is time to do the shutdown.
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

    /// <inheritdoc />
    public void Run()
    {
        if (IsShutdownComplete)
            throw new ObjectDisposedException(GetType().FullName, Strings.ExecutorIsDisposed);

        lock (Lock)
        {
            if (Window != null)
                throw new InvalidOperationException(Strings.ExecutorAlreadyRunning);

            Thread = Thread.CurrentThread;
            Window = new MessageOnlyWindowWrapper(this);

            Window.AddCallback(_callback);

            _running.Set();
            // A call to Dispose() may have been made (either deliberately or due to a using statement/declaration) before
            // or during this initialization method. If that's the case, we queue the disposal so that it'll happen as soon
            // as this executor begins running.
            if (_disposeQueued)
                InvokeAsync(Dispose);
        }

        PushFrame(CreateFrame(true));
    }

    /// <inheritdoc/>
    public ThreadExecutorOperation StartAsync()
    {
        if (Window != null)
        {   // We fail immediately if the executor is already running; as it becomes difficult to report errors to awaiting callers.
            throw new InvalidOperationException(Strings.ExecutorAlreadyRunning);
        }

        // Run() blocks the calling thread until the executor is shut down, so we must offload to another thread in order
        // for this to return.
        var runTask = Task.Run(Run);

        // A no-op InvokeAsync will return an operation that will complete whenever the executor begins running and
        // processes the request.
        ThreadExecutorOperation operation = InvokeAsync(() => { });

        // Because the initialization task never returns (until the executor is shut down), we cannot await it.
        // If any exceptions occur during execution of this task, we'll need to send them through the no-op operation
        // (which will be awaited) instead via its task source.
        // Given that the no-op operation cannot execute until the offloaded task finishes initialization, the following
        // continuation task (which will only run if an error occurs during initialization) is guaranteed to run before it.
        runTask.ContinueWith(t =>
                             {
                                 // The 'await' keyword normally unwraps the AggregateException and throws the inner exception
                                 // instead. We will do the same here, via the task source.
                                 if (t.Exception != null)
                                     operation.TaskSource.SetException(t.Exception.InnerExceptions[0]);
                             },
                             CancellationToken.None,
                             TaskContinuationOptions.OnlyOnFaulted,
                             TaskScheduler.Default);
        return operation;
    }

    /// <inheritdoc/>
    public SynchronizationContext? SwitchContext()
    {
        var oldContext = SynchronizationContext.Current;
        var newContext = new ThreadExecutorContext(this);

        SynchronizationContext.SetSynchronizationContext(newContext);

        return oldContext;
    }

    /// <inheritdoc/>
    public int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
    {
        Require.NotNull(waitHandles, nameof(waitHandles));

        return Kernel32.WaitForMultipleObjectsEx(waitHandles.Length,
                                                 waitHandles,
                                                 waitAll,
                                                 (uint) millisecondsTimeout,
                                                 false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsShutdownComplete)
            return;
        
        lock (Lock)
        {
            if (!_hasStarted)
            {   // The executor isn't running yet. Setting this flag here will ensure that disposal will happen even if Run() is
                // executing concurrent to this.
                _disposeQueued = true;
                return;
            }
        }

        if (_framesRunning == 0)
            StartShutdown();
        else
            Invoke(StartShutdown);

        _shutdownContext?.Dispose();
        _running.Dispose();

        lock (_ExecutorsLock)
        {
            _Executors.Remove(_thisExecutor);
        }
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

    private bool OnExecutorThread()
    {   // The executor's thread isn't finalized until it is up and running, so here we wait.
        _running.Wait();

        return Thread == Thread.CurrentThread;
    }

    private object? InvokeContext(Func<object?> method)
    {
        SynchronizationContext? oldContext = SwitchContext();

        try
        {
            return method();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(oldContext);
        }
    }

    private MessageOnlyExecutorFrame CreateFrame(bool exitUponRequest)
        => new (this, exitUponRequest);

    private void InvokeAsync(ThreadExecutorOperation operation)
    {
        bool succeeded = false;

        _running.Wait();

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

    private ProcedureResult WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var message = (WindowMessage) msg;

        if (DisableRequests <= 0)
        {
            if (WindowMessage.Destroy == message)
            {
                if (!IsShutdownStarted && !IsShutdownComplete)
                    Shutdown();
            }

            else if (_ProcessOperation == message)
            {
                ProcessOperation();
            }
        }

        return new ProcedureResult(IntPtr.Zero, false);
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
