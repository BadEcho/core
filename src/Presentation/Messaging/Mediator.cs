//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Threading;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Messaging;

/// <summary>
/// Provides loosely-coupled messaging between various colleagues with weakly referenced mapped action support.
/// </summary>
public sealed class Mediator : DispatcherObject
{
    private readonly CallbackMessageMap _map = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    public Mediator()
    {
        if (null == Dispatcher)
            throw new InvalidOperationException(Strings.MediatorDispatcherNotAccessible);
    }

    /// <summary>
    /// Broadcasts to all colleagues that the provided message has been issued.
    /// </summary>
    /// <param name="message">The message that has been issued.</param>
    public void Broadcast(MediatorMessage message)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Action> actions = _map.GetActions(message);

        foreach (var action in actions)
        {
            action();
        }
    }

    /// <summary>
    /// Broadcasts to the first colleague receiving it that the provided message has been issued.
    /// </summary>
    /// <param name="message">The message that has been issued.</param>
    public void BroadcastSingle(MediatorMessage message)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Action> actions = _map.GetActions(message);
        var action = actions.FirstOrDefault();

        action?.Invoke();
    }

    /// <summary>
    /// Broadcasts to all colleagues that the provided message has been issued.
    /// </summary>
    /// <typeparam name="T">The type of parameter accepted by the mapped callbacks.</typeparam>
    /// <param name="message">The message that has been issued.</param>
    /// <param name="parameter">The parameter to provide to the mapped callbacks.</param>
    public void Broadcast<T>(MediatorMessage message, T parameter)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Action<T>> actions = _map.GetActions<T>(message);

        foreach (var action in actions)
        {
            action(parameter);
        }
    }

    /// <summary>
    /// Broadcasts to the first colleague receiving it that the provided message has been issued.
    /// </summary>
    /// <typeparam name="T">The type of parameter accepted by the mapped callbacks.</typeparam>
    /// <param name="message">The message that has been issued.</param>
    /// <param name="parameter">The parameter to provide to the mapped callbacks.</param>
    public void BroadcastSingle<T>(MediatorMessage message, T parameter)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Action<T>> actions = _map.GetActions<T>(message);

        var action = actions.FirstOrDefault();

        action?.Invoke(parameter);
    }

    /// <summary>
    /// Broadcasts to all colleagues that the provided message has been issued, receiving all responses to the
    /// broadcast.
    /// </summary>
    /// <typeparam name="T">The type of result returned by broadcast responses.</typeparam>
    /// <param name="message">The message that has been issued.</param>
    /// <returns>A sequence of all non-null responses to the broadcast.</returns>
    /// <remarks>
    /// Responses returning null values are disregarded as null values within a sequence often offer no value on their
    /// own, and it also provides a way for responders to opt-out of having to provide something if they have nothing meaningful
    /// to offer at the moment of message issuance.
    /// </remarks>
    public IEnumerable<T> BroadcastReceive<T>(MediatorMessage message)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Func<T>> functions = _map.GetFunctions<T>(message);

        return functions.Select(function => function()).WhereNotNull();
    }

    /// <summary>
    /// Broadcasts to all colleagues that the provided message has been issued, receiving all responses to the
    /// broadcast.
    /// </summary>
    /// <typeparam name="TInput">The type of parameter accepted by the mapped callbacks.</typeparam>
    /// <typeparam name="TOutput">The type of result returned by broadcast responses.</typeparam>
    /// <param name="message">The message that has been issued.</param>
    /// <param name="parameter">The parameter to provide to the mapped callbacks.</param>
    /// <returns>A sequence of all non-null responses to the broadcast.</returns>
    /// <remarks>
    /// Responses returning null values are disregarded as null values within a sequence often offer no value on their
    /// own, and it also provides a way for responders to opt-out of having to provide something if they have nothing meaningful
    /// to offer at the moment of message issuance.
    /// </remarks>
    public IEnumerable<TOutput> BroadcastReceive<TInput, TOutput>(MediatorMessage message, TInput parameter)
    {
        Require.NotNull(message, nameof(message));

        IEnumerable<Func<TInput, TOutput>> functions = _map.GetFunctions<TInput, TOutput>(message);

        return functions.Select(function => function(parameter)).WhereNotNull();
    }

    /// <summary>
    /// Registers support for the provided message by mapping it to the provided action.
    /// </summary>
    /// <param name="message">The message to associate the callback with.</param>
    /// <param name="callback">The callback to execute when the message is broadcast.</param>
    public void Register(MediatorMessage message, MulticastDelegate callback)
    {
        Require.NotNull(message, nameof(message));
        Require.NotNull(callback, nameof(callback));

        _map.AddMapping(message, callback);
    }

    /// <summary>
    /// Removes support for a previously supported message and callback.
    /// </summary>
    /// <param name="message">The message associated with the callback.</param>
    /// <param name="callback">The callback to no longer execute when the message is broadcast.</param>
    public void Unregister(MediatorMessage message, MulticastDelegate callback)
    {
        Require.NotNull(message, nameof(message));
        Require.NotNull(callback, nameof(callback));

        _map.RemoveMapping(message, callback);
    }

    /// <summary>
    /// Removes support for a previously supported message entirely.
    /// </summary>
    /// <param name="message">The message to disassociate with all previously registered callbacks.</param>
    public void Unregister(MediatorMessage message)
    {
        Require.NotNull(message, nameof(message));
        
        _map.RemoveMappings(message);
    }

    /// <summary>
    /// Removes support for all messages previously registered with this mediator.
    /// </summary>
    public void UnregisterAll() 
        => _map.RemoveMappings();
}
