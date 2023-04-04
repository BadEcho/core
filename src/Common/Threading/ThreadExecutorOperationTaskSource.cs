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

using System.Diagnostics.CodeAnalysis;
using BadEcho.Properties;

namespace BadEcho.Threading;

/// <summary>
/// Provides a source for a <see cref="TaskCompletionSource{TResult}"/> for a particular <see cref="ThreadExecutorOperation"/>.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the operation.</typeparam>
internal class ThreadExecutorOperationTaskSource<TResult> : IThreadExecutorOperationTaskSource
{
    private TaskCompletionSource<TResult?>? _source;

    /// <inheritdoc/>
    public Task Task
    {
        get
        {
            EnsureInitialized();

            return _source.Task;
        }
    }

    /// <inheritdoc/>
    public void Initialize(ThreadExecutorOperation operation)
    {
        if (_source != null)
            throw new InvalidOperationException(Strings.ThreadExecutorSourceAlreadyInitialized);

        _source = new TaskCompletionSource<TResult?>(operation);
    }

    /// <inheritdoc/>
    public void SetCanceled()
    {
        EnsureInitialized();

        _source.SetCanceled();
    }

    /// <inheritdoc/>
    public void SetResult(object? result)
    {
        EnsureInitialized();

        _source.SetResult((TResult?) result);
    }

    /// <inheritdoc/>
    public void SetException(Exception exception)
    {
        EnsureInitialized();

        _source.SetException(exception);
    }

    [MemberNotNull(nameof(_source))]
    private void EnsureInitialized()
    {
        if (_source == null)
            throw new InvalidOperationException(Strings.ThreadExecutorSourceNotInitialized);
    }
}