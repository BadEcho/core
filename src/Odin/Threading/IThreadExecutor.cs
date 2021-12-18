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

namespace BadEcho.Odin.Threading;

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
    TResult Invoke<TResult>(Func<TResult> method);

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
    ThreadExecutorOperation<TResult> InvokeAsync<TResult>(Func<TResult> method);

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
    /// Directly executes the provided method synchronously using the executor's context and thread.
    /// </summary>
    /// <param name="method">The method to directly execute using the executor's context.</param>
    /// <param name="filterExceptions">
    /// Value indicating if the method should be executed in a wrapped context, allowing the executor to catch and filter
    /// any exceptions thrown, as opposed to simply executing the method and allowing for normal exception flow to occur.
    /// </param>
    /// <param name="args">Optional arguments to pass to the given method.</param>
    /// <returns>The result of the execution.</returns>
    /// <remarks>
    /// This is where the actual execution of any provided method occurs, meant to be called eventually by all invocation
    /// routines exposed by the executor. If this is called on a thread other than the one associated with the executor, the
    /// method will be queued for execution on said thread.
    /// </remarks>
    internal object Invoke(Delegate method, bool filterExceptions, object? args);

    /// <summary>
    /// Directly executes the provided method asynchronously using the executor's context and thread.
    /// </summary>
    /// <param name="method">The method to directly execute using the executor's context.</param>
    /// <param name="filterExceptions">
    /// Value indicating if the method should be executed in a wrapped context, allowing the executor to catch and filter
    /// any exceptions thrown, as opposed to simply executing the method and allowing for normal exception flow to occur.
    /// </param>
    /// <param name="args">Optional arguments to pass to the given method.</param>
    /// <remarks>
    /// This is where provided methods are queued for posting to the executor's particular message pump in order for their
    /// eventual execution on the thread associated with the executor.
    /// </remarks>
    internal void BeginInvoke(Delegate method, bool filterExceptions, object? args);
}