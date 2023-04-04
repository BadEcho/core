//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Threading;

/// <summary>
/// Defines an entity responsible for the execution of an action through a particular thread or context.
/// </summary>
public interface IThreadExecutor
{
    /// <summary>
    /// Gets a value indicating if the process to shutdown the executor has started.
    /// </summary>
    bool IsShutdownStarted { get; }

    /// <summary>
    /// Gets a value indicating if the executor is no longer running.
    /// </summary>
    bool IsShutdownComplete { get; }

    /// <summary>
    /// Gets the number of requests made to the executor to disable all processing of queued actions.
    /// </summary>
    int DisableRequests { get; }

    /// <summary>
    /// Gets the object used to synchronize operational-related changes to the executor.
    /// </summary>
    object Lock { get; }

    /// <summary>
    /// Gets the thread that the executor is running on.
    /// </summary>
    Thread Thread { get; }

    /// <summary>
    /// Executes the provided <see cref="Action"/> synchronously on the thread the executor is associated with.
    /// </summary>
    /// <param name="method">The method to invoke through the executor.</param>
    void Invoke(Action method);

    /// <summary>
    /// Executes the provided <see cref="Func{TResult}"/> synchronously on the thread the executor is associated with.
    /// </summary>
    /// <typeparam name="TResult">The return type of the specified method.</typeparam>
    /// <param name="method">The method to invoke through the executor.</param>
    /// <returns>The value returned by <c>method</c>.</returns>
    TResult? Invoke<TResult>(Func<TResult> method);

    /// <summary>
    /// Executes the provided <see cref="Action"/> asynchronously on the thread the executor is associated with.
    /// </summary>
    /// <param name="method">The method to invoke through the executor. </param>
    /// <returns>
    /// A <see cref="ThreadExecutorOperation"/> representing the asynchronous operation that can be interacted with as it is pending
    /// execution.
    /// </returns>
    ThreadExecutorOperation InvokeAsync(Action method);

    /// <summary>
    /// Executes the provided <see cref="Func{TResult}"/> asynchronously on the thread the executor is associated with.
    /// </summary>
    /// <typeparam name="TResult">The return type of the specified method.</typeparam>
    /// <param name="method">The method to invoke through the executor.</param>
    /// <returns>
    /// A <see cref="ThreadExecutorOperation{TResult}"/> representing the asynchronous operation that can be interacted with as it is
    /// pending execution.
    /// </returns>
    ThreadExecutorOperation<TResult?> InvokeAsync<TResult>(Func<TResult> method);

    /// <summary>
    /// Cancels the provided executor operation.
    /// </summary>
    /// <param name="operation">The operation to cancel.</param>
    /// <returns>True if <c>operation</c> was canceled; otherwise, false.</returns>
    bool Cancel(ThreadExecutorOperation operation);

    /// <summary>
    /// Disables the processing of requests by the executor.
    /// </summary>
    void Disable();

    /// <summary>
    /// Re-enables the processing of requests by the executor.
    /// </summary>
    void Enable();

    /// <summary>
    /// Creates a new thread executor frame.
    /// </summary>
    /// <returns>The newly created frame.</returns>
    IThreadExecutorFrame CreateFrame();

    /// <summary>
    /// Pushes the provided executor frame into processing.
    /// </summary>
    /// <param name="frame">The frame to process.</param>
    void PushFrame(IThreadExecutorFrame frame);

    /// <summary>
    /// Pushes the main execution frame onto the executor and starts processing requests.
    /// </summary>
    /// <remarks>
    /// This will start the executor to process requests on the thread invoking it; it will not return
    /// (block) until the executor is shutdown.
    /// </remarks>
    void Run();

    /// <summary>
    /// Begins initialization of the executor to process requests on another thread, resuming execution of
    /// the calling <c>async</c> method once the executor is running and ready to receive requests.
    /// </summary>
    /// <returns>
    /// A <see cref="ThreadExecutorOperation"/> representing the asynchronous initialization operation that
    /// can be awaited.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This call offloads the initialization to another thread because not doing so would result in the
    /// executor processing requests for the current thread until its explicit shutdown. Because of this, it
    /// would never release control back to the caller.
    /// </para>
    /// <para> 
    /// A direct call to <see cref="Run"/> can be made in order start the executor on the current thread instead
    /// (which will, of course, not return until the executor is shutdown).
    /// </para>
    /// </remarks>
    ThreadExecutorOperation RunAsync();

    /// <summary>
    /// Switches the current synchronization context to that executor's own.
    /// </summary>
    /// <returns>The <see cref="SynchronizationContext"/> instance previously set as the synchronization context.</returns>
    SynchronizationContext? SwitchContext();

    /// <summary>
    /// Executes the provided delegate synchronously using the executor's context and thread.
    /// </summary>
    /// <param name="method">The method to directly execute using the executor's context.</param>
    /// <param name="argument">Optional argument to pass to the given method.</param>
    /// <returns>The result of the execution.</returns>
    internal object? Invoke(Delegate method, object? argument);

    /// <summary>
    /// Executes the provided method asynchronously using the executor's context and thread.
    /// </summary>
    /// <param name="method">The method to directly execute using the executor's context.</param>
    /// <param name="argument">Optional argument to pass to the given method.</param>
    internal void BeginInvoke(Delegate method, object? argument);
}