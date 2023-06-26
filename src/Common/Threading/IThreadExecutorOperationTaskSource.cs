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
/// Defines a source for a <see cref="TaskCompletionSource{TResult}"/> for a particular <see cref="ThreadExecutorOperation"/>.
/// </summary>
internal interface IThreadExecutorOperationTaskSource
{
    /// <summary>
    /// Gets the task for the operation.
    /// </summary>
    Task Task { get; }
    
    /// <summary>
    /// Initializes the underlying source to use the provided operation as its asynchronous state.
    /// </summary>
    /// <param name="operation">The operation to use as the underlying source's asynchronous state.</param>
    /// <exception cref="InvalidOperationException">Task source is already initialized.</exception>
    void Initialize(ThreadExecutorOperation operation);

    /// <summary>
    /// Transitions the underlying task into a canceled state.
    /// </summary>
    void SetCanceled();

    /// <summary>
    /// Transitions the underlying task into a completed state.
    /// </summary>
    /// <param name="result">The result of the operation.</param>
    void SetResult(object? result);

    /// <summary>
    /// Transitions the underlying task into a faulted state.
    /// </summary>
    /// <param name="exception">The exception that caused the task to fail.</param>
    void SetException(Exception exception);
}
