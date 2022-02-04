//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Threading;

/// <summary>
/// Provides a representation of a delegate that has been posted to an executor's queue.
/// </summary>
public class ThreadExecutorOperation
{
    private static readonly ContextCallback _ContextCallback = ContextCallback;
    
    private EventHandler? _canceled;
    private EventHandler? _completed;

    private readonly Delegate _method;
    private readonly object? _argument;
    private readonly bool _filterExceptions;

    private ExecutionContext? _context;
    private Exception? _exception;
    private object? _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorOperation"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the operation.</param>
    /// <param name="method">An action, that takes no arguments, being executed.</param>
    /// <param name="filterExceptions">
    /// Value indicating if the method should be executed in a wrapped context, allowing the executor to catch and filter
    /// any exceptions thrown, as opposed to simply executing the method and allowing for normal exception flow to occur.
    /// </param>
    internal ThreadExecutorOperation(IThreadExecutor executor, Action method, bool filterExceptions)
        : this(executor, method, filterExceptions, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorOperation"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the operation.</param>
    /// <param name="method">The method being executed.</param>
    /// <param name="filterExceptions">
    /// Value indicating if the method should be executed in a wrapped context, allowing the executor to catch and filter
    /// any exceptions thrown, as opposed to simply executing the method and allowing for normal exception flow to occur.
    /// </param>
    /// <param name="argument">The argument to provide to the method.</param>
    internal ThreadExecutorOperation(IThreadExecutor executor,
                                     Delegate method,
                                     bool filterExceptions,
                                     object? argument)
        : this(executor, method, filterExceptions, argument, new ThreadExecutorOperationTaskSource<object>())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorOperation"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the operation.</param>
    /// <param name="method">The method being executed.</param>
    /// <param name="filterExceptions">
    /// Value indicating if the method should be executed in a wrapped context, allowing the executor to catch and filter
    /// any exceptions thrown, as opposed to simply executing the method and allowing for normal exception flow to occur.
    /// </param>
    /// <param name="argument">The argument to provide to the method.</param>
    /// <param name="taskSource">The task completion source for the operation.</param>
    internal ThreadExecutorOperation(IThreadExecutor executor,
                                     Delegate method,
                                     bool filterExceptions,
                                     object? argument,
                                     IThreadExecutorOperationTaskSource taskSource)
    {
        Executor = executor;
        TaskSource = taskSource;

        _method = method;
        _argument = argument;
        _filterExceptions = filterExceptions;

        _context = ExecutionContext.Capture();
    }

    /// <summary>
    /// Occurs when the operation is canceled.
    /// </summary>
    public event EventHandler Canceled
    {
        add
        {
            lock (ExecutorLock)
            {
                _canceled = (EventHandler?) Delegate.Combine(_canceled, value);
            }
        }
        remove
        {
            lock (ExecutorLock)
            {
                _canceled = (EventHandler?) Delegate.Remove(_canceled, value);
            }
        }
    }

    /// <summary>
    /// Occurs when the operation is completed.
    /// </summary>
    public event EventHandler Completed
    {
        add
        {
            lock (ExecutorLock)
            {
                _completed = (EventHandler?) Delegate.Combine(_completed, value);
            }
        }
        remove
        {
            lock (ExecutorLock)
            {
                _completed = (EventHandler?) Delegate.Combine(_completed, value);
            }
        }
    }

    /// <summary>
    /// Gets the executor powering the operation.
    /// </summary>
    public IThreadExecutor Executor
    { get; }

    /// <summary>
    /// Gets the status of the operation.
    /// </summary>
    public ThreadExecutorOperationStatus Status
    { get; internal set; }

    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    public object? Result
    {
        get
        {
            // We'll want to wait for the operation to complete, if necessary, before returning the result, to allow for the task to
            // throw any captured exceptions.
            // This is something we'll want to do if running in an async context, something which only can occur if we're not 
            // filtering exceptions. Filtered exception invocation is functionality that not publicly available to external callers,
            // rather it is something used only by legacy/internal APIs (such as SynchronizationContext's Send/Post and the message
            // pump), which never run in an async context.
            if (!_filterExceptions)
            {
                Wait();

                if (Status is ThreadExecutorOperationStatus.Completed or ThreadExecutorOperationStatus.Canceled)
                    Task.GetAwaiter().GetResult();
            }

            return _result;
        }
    }

    /// <summary>
    /// Gets the task representing the operation.
    /// </summary>
    public Task Task
        => TaskSource.Task;

    /// <summary>
    /// Gets the task completion source for the operation.
    /// </summary>
    internal IThreadExecutorOperationTaskSource TaskSource
    { get; }
    
    /// <summary>
    /// Gets the executor's synchronization object.
    /// </summary>
    private object ExecutorLock
        => Executor.Lock;

    /// <summary>
    /// Gets an awaiter for awaiting the completion of the operation.
    /// </summary>
    /// <returns>A <see cref="TaskAwaiter"/> value for awaiting the completion of the operation.</returns>
    /// <remarks>This provides async/await support for the <see cref="ThreadExecutorOperation"/> type.</remarks>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TaskAwaiter GetAwaiter()
        => Task.GetAwaiter();

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    /// <returns>Value indicating if the operation was removed from processing.</returns>
    public bool Cancel()
    {
        bool removed = Executor.Cancel(this);

        if (removed)
        {
            TaskSource.SetCanceled();

            _canceled?.Invoke(this, EventArgs.Empty);
        }

        return removed;
    }

    /// <summary>
    /// Waits until the operation completes.
    /// </summary>
    /// <returns>The status of the operation.</returns>
    public ThreadExecutorOperationStatus Wait()
        => Wait(TimeSpan.FromMilliseconds(-1));

    /// <summary>
    /// Waits until the operation completes.
    /// </summary>
    /// <param name="timeout">The maximum amount of time to wait.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// While on the native executor's thread, an attempt to wait was made on a running operation.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The operation is expected to be in a running or pending state when executing this method, otherwise nothing will happen
    /// and this will return immediately. If the operation is running or pending, then the actions taken by this method depend
    /// on whether the current thread is the same as the native executor's owning thread.
    /// </para>
    /// <para>
    /// If the current thread is the thread that the native executor was created on, then the operation must be pending. If the
    /// operation is already running, then an exception is thrown in order to avoid a deadlock. That aside, the operation is
    /// essentially jump started by this method by creating a <see cref="ThreadExecutorOperationFrame"/> for the operation and then
    /// pushing it onto the executor via <see cref="IThreadExecutor.PushFrame"/>. This will have the effect of working the message
    /// queue so that the queued operation is ultimately processed by the executor.
    /// </para>
    /// <para>
    /// If the current thread differs from the thread that the native executor was created on, then we are in the clear to simply
    /// block. This is done by creating a <see cref="ThreadExecutorOperationEvent"/>, which is essentially a decorated
    /// <see cref="ManualResetEvent"/>, and then waiting on that.
    /// </para>
    /// </remarks>
    public ThreadExecutorOperationStatus Wait(TimeSpan timeout)
    {
        bool operationExecuting = Status is ThreadExecutorOperationStatus.Running or ThreadExecutorOperationStatus.Pending
            && !timeout.TotalMilliseconds.ApproximatelyEquals(0.0);

        if (operationExecuting)
        {
            if (Thread.CurrentThread == Executor.Thread)
            {
                if (ThreadExecutorOperationStatus.Running == Status)
                    throw new InvalidOperationException(Strings.ExecutorWaitOperationSameThread);

                IThreadExecutorFrame innerFrame = Executor.CreateFrame();

                using (var operationFrame = new ThreadExecutorOperationFrame(innerFrame, this, timeout))
                {
                    Executor.PushFrame(operationFrame);
                }
            }
            else
            {
                using (var waitEvent = new ThreadExecutorOperationEvent(this, timeout))
                {
                    waitEvent.WaitOne();
                }
            }
        }

        // This is to give the task a chance to throw any captured exceptions, something we only want to do if not filtering the exceptions
        // and potentially running in an async context.
        if (!_filterExceptions && Status is ThreadExecutorOperationStatus.Completed or ThreadExecutorOperationStatus.Canceled)
            Task.GetAwaiter().GetResult();

        return Status;
    }

    internal void Invoke()
    {
        Status = ThreadExecutorOperationStatus.Running;

        if (_context != null)
        {
            ExecutionContext.Run(_context, _ContextCallback, this);

            _context.Dispose();
            _context = null;
        }
        else
            _ContextCallback(this);

        EventHandler? resultHandler;

        lock (ExecutorLock)
        {   
            if (_exception is OperationCanceledException)
            {   // Tasks may throw OperationCanceledException when canceled or aborted, we'll reflect this in the status.
                resultHandler = _canceled;
                Status = ThreadExecutorOperationStatus.Canceled;
            }
            else
            {   // Otherwise the operation is marked as completed, whether or not an exception was thrown. Exception information
                // is made available when finalizing the completion.
                resultHandler = _completed;
                Status = ThreadExecutorOperationStatus.Completed;
            }
        }

        resultHandler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Propagates a completed operation's status to its associated task.
    /// </summary>
    internal void FinalizeCompletion()
    {
        switch (Status)
        {
            case ThreadExecutorOperationStatus.Canceled:
                TaskSource.SetCanceled();
                break;
            case ThreadExecutorOperationStatus.Completed:
                if (_exception != null)
                    TaskSource.SetException(_exception);
                else
                    TaskSource.SetResult(Result);
                break;
            default:
                throw new InvalidOperationException(Strings.ExecutorFinalizedBeforeDone);
        }
    }

    private static void ContextCallback(object? state)
    {
        var operation = (ThreadExecutorOperation?) state;

        operation?.Execute();
    }

    private void Execute()
    {
        // If we're filtering exceptions, then we'll let the executor's invoke filter handle (or not handle) any thrown
        // exception.
        if (_filterExceptions)
        {
            _result = Executor.Invoke(_method, true, _argument);
        }
        else
        {
            try
            {
                _result = Executor.Invoke(_method, false, _argument);
            }
            catch (Exception ex)
            {   // This will be reported through the task completion source.
                _exception = ex;
            }
        }
    }

    /// <summary>
    /// Provides a representation of a thead executor operation frame.
    /// </summary>
    private sealed class ThreadExecutorOperationFrame : IThreadExecutorFrame, IDisposable
    {
        private readonly IThreadExecutorFrame _innerFrame;
        private readonly ThreadExecutorOperation _operation;
        private readonly Timer? _timer;

        private bool _isExited;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadExecutorOperationFrame"/> class.
        /// </summary>
        /// <param name="innerFrame">Generated frame that supplies execution continuation logic.</param>
        /// <param name="operation">The thread executor operation.</param>
        /// <param name="timeout">The maximum amount of time to wait.</param>
        public ThreadExecutorOperationFrame(IThreadExecutorFrame innerFrame, ThreadExecutorOperation operation, TimeSpan timeout)
        {
            _innerFrame = innerFrame;
            _operation = operation;
            _isExited = false;

            _operation.Canceled += HandleOperationCanceledOrCompleted;
            _operation.Completed += HandleOperationCanceledOrCompleted;

            if (timeout.TotalMilliseconds > 0)
            {
                _timer = new Timer(HandleTimerTick,
                                   null,
                                   timeout,
                                   TimeSpan.FromMilliseconds(-1));
            }

            ExitUponRequest = false;

            if (ThreadExecutorOperationStatus.Pending != _operation.Status)
                Exit();
        }

        /// <inheritdoc/>
        public bool ExitUponRequest
        {
            get => _innerFrame.ExitUponRequest;
            set => _innerFrame.ExitUponRequest = value;
        }

        /// <inheritdoc/>
        public bool ShouldContinue
        {
            get => _innerFrame.ShouldContinue;
            set => _innerFrame.ShouldContinue = value;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _timer?.Dispose();

            _operation.Canceled -= HandleOperationCanceledOrCompleted;
            _operation.Completed -= HandleOperationCanceledOrCompleted;
        }

        private void Exit()
        {
            if (_isExited)
                return;

            ShouldContinue = false;
            _isExited = true;
        }
        
        private void HandleOperationCanceledOrCompleted(object? sender, EventArgs e)
        {
            Exit();
        }

        private void HandleTimerTick(object? state)
        {
            Exit();
        }
    }

    /// <summary>
    /// Provides a representation of a manual reset event for a thread executor operation.
    /// </summary>
    private sealed class ThreadExecutorOperationEvent : IDisposable
    {
        private readonly ManualResetEvent _event;
        private readonly ThreadExecutorOperation _operation;
        private readonly TimeSpan _timeout;

        private bool _isClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadExecutorOperationEvent"/> class.
        /// </summary>
        /// <param name="operation">The thread executor operation.</param>
        /// <param name="timeout">The maximum amount of time to wait.</param>
        public ThreadExecutorOperationEvent(ThreadExecutorOperation operation, TimeSpan timeout)
        {
            _operation = operation;
            _timeout = timeout;

            _event = new ManualResetEvent(false);
            _isClosed = false;

            lock (OperationLock)
            {
                _operation.Canceled += HandleOperationCanceledOrCompleted;
                _operation.Completed += HandleOperationCanceledOrCompleted;
                // We'll only want to block for operations that haven't been completed or aborted.
                if (_operation.Status is not ThreadExecutorOperationStatus.Pending and not ThreadExecutorOperationStatus.Running)
                    _event.Set();
            }
        }

        /// <summary>
        /// Gets the synchronization object used by the operation.
        /// </summary>
        private object OperationLock
            => _operation.ExecutorLock;

        /// <summary>
        /// Waits for the event.
        /// </summary>
        /// <suppressions>
        /// ReSharper disable InconsistentlySynchronizedField
        /// We synchronize access to _isClosed and _operation, we actually don't care about synchronizing access to _event.
        /// </suppressions>
        public void WaitOne()
            => _event.WaitOne(_timeout, false);

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (OperationLock)
            {
                if (_isClosed)
                    return;

                _operation.Canceled -= HandleOperationCanceledOrCompleted;
                _operation.Canceled -= HandleOperationCanceledOrCompleted;

                _event.Close();
                
                _isClosed = true;
            }
        }

        private void HandleOperationCanceledOrCompleted(object? sender, EventArgs e)
        {
            lock (OperationLock)
            {
                if (!_isClosed)
                    _event.Set();
            }
        }
    }
}