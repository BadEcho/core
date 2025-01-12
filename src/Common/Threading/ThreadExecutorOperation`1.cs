//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BadEcho.Threading;

/// <summary>
/// Provides a representation of a delegate that has been posted to an executor's queue.
/// </summary>
/// <typeparam name="TResult">The return type of the delegate encapsulated by the operation.</typeparam>
/// <remarks>
/// <para>
/// We see a lot of method and property hiding instead of proper overloading in this class as there is really
/// no other choice, given the fact that <see cref="TaskAwaiter{TResult}"/> does not, and indeed cannot derive
/// from <see cref="TaskAwaiter"/>.
/// </para>
/// <para>
/// Each method and property hidden by this class simply engages in upcasting the base value, so there is really
/// no functional change between this type and its base (save for the GetAwaiter method, although the base form of
/// it should result in the same outcome), and therefore there should be no risk of unexpected behavior should this
/// type ever be handled in its base form.
/// </para>
/// </remarks>
public class ThreadExecutorOperation<TResult> : ThreadExecutorOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorOperation"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the operation.</param>
    /// <param name="method">The method being executed.</param>
    internal ThreadExecutorOperation(IThreadExecutor executor, Func<TResult> method)
        : base(executor, method, false, null, new ThreadExecutorOperationTaskSource<TResult>())
    { }

    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    public new TResult? Result
        => (TResult?) base.Result;

    /// <summary>
    /// Gets the task that's representing the operation and able to return a value.
    /// </summary>
    public new Task<TResult> Task
        => (Task<TResult>)base.Task;

    /// <summary>
    /// Gets an awaiter for awaiting the completion of the operation.
    /// </summary>
    /// <returns>A <see cref="TaskAwaiter{TResult}"/> value for awaiting the completion of the operation.</returns>
    /// <remarks>This provides async/await support for the <see cref="ThreadExecutorOperation{TResult}"/> type.</remarks>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new TaskAwaiter<TResult> GetAwaiter()
        => Task.GetAwaiter();

    /// <inheritdoc/>
    protected override object? InvokeMethod()
    {
        Func<TResult> function = (Func<TResult>) Method;

        return function();
    }
}