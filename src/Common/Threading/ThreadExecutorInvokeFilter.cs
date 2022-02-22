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

namespace BadEcho.Threading;

/// <summary>
/// Provides method execution routines designed specifically for thread executor style invocations, optionally offering exception
/// filtering.
/// </summary>
internal sealed class ThreadExecutorInvokeFilter
{
    private readonly object _source;

    /// <summary>
    /// Occurs when an exception has been thrown and is available for filtering by subscribers.
    /// </summary>
    public event EventHandler<ThreadExceptionEventArgs>? Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExecutorInvokeFilter"/> class.
    /// </summary>
    /// <param name="source">The object using this wrapper.</param>
    public ThreadExecutorInvokeFilter(object source)
    {
        _source = source;
    }

    /// <summary>
    /// Executes the provided delegate in the most efficient way possible, optionally filtering out any thrown
    /// exceptions if requested.
    /// </summary>
    /// <param name="method">The delegate to execute.</param>
    /// <param name="filterExceptions">Value indicating if exceptions should be filtered out.</param>
    /// <param name="argument">An optional argument to pass to the delegate.</param>
    /// <returns>The result of executing <c>method</c>.</returns>
    public object? Execute(Delegate method, bool filterExceptions, object? argument)
    {
        Require.NotNull(method, nameof(method));
        object? result = null;

        try
        {
            result = Execute(method, argument);
        }
        catch (Exception ex)
        {
            if (!filterExceptions || !FilterException(ex))
                throw;
        }

        return result;
    }

    private static object? Execute(Delegate method, object? argument)
    {
        Require.NotNull(method, nameof(method));

        object? result = null;

        switch (method)
        {
            case Action action:
                action();
                break;

            case Func<object> function:
                result = function();
                break;

            case ThreadExecutorOperationCallback function:
                result = function(argument);
                break;

            case SendOrPostCallback callback:
                callback(argument);
                break;

            default:
                method.DynamicInvoke(argument);
                break;
        }

        return result;
    }

    private bool FilterException(Exception exception)
    {
        EventHandler<ThreadExceptionEventArgs>? filter = Filter;

        if (filter == null)
            return false;

        var args = new ThreadExceptionEventArgs(exception);

        filter(_source, args);

        return args.Handled;
    }
}
