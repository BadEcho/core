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

using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Messaging;

/// <summary>
/// Provides a container for mappings between mediated messages and associated callbacks.
/// </summary>
internal sealed class CallbackMessageMap
{
    private readonly Dictionary<MediatorMessage, IList<WeakCallback>> _mappings
        = new();

    /// <summary>
    /// Creates a mapping between the message and the callback.
    /// </summary>
    /// <param name="message">The message to associate the callback with.</param>
    /// <param name="callback">The callback to associate with the message.</param>
    public void AddMapping(MediatorMessage message, MulticastDelegate callback)
    {
        var callbackType = callback.GetType();

        EnsureCompatibleCallback(message, callbackType);

        var weakCallback = new WeakCallback(callback);
        IList<WeakCallback> callbacksForMessage = GetCallbacksForMessage(message);

        if (!callbacksForMessage.Contains(weakCallback))
            callbacksForMessage.Add(weakCallback);
    }

    /// <summary>
    /// Retrieves the actions associated with the provided message.
    /// </summary>
    /// <param name="message">A message mapped to actions.</param>
    /// <returns>A sequence of all the actions associated with <c>message</c>.</returns>
    public IEnumerable<Action> GetActions(MediatorMessage message)
        => CollectCallbacks<Action>(message);

    /// <summary>
    /// Retrieves the actions associated with the provided message.
    /// </summary>
    /// <typeparam name="T">The type of parameter accepted by the actions.</typeparam>
    /// <param name="message">A message mapped to actions.</param>
    /// <returns>A sequence of all the actions associated with <c>message</c>.</returns>
    public IEnumerable<Action<T>> GetActions<T>(MediatorMessage message)
        => CollectCallbacks<Action<T>>(message);

    /// <summary>
    /// Retrieves the functions associated with the provided message.
    /// </summary>
    /// <typeparam name="T">The type of parameter accepted by the functions.</typeparam>
    /// <param name="message">A message mapped to functions.</param>
    /// <returns>A sequence of all functions associated with <c>message</c>.</returns>
    public IEnumerable<Func<T>> GetFunctions<T>(MediatorMessage message)
        => CollectCallbacks<Func<T>>(message);

    /// <summary>
    /// Retrieves the functions associated with the provided message.
    /// </summary>
    /// <typeparam name="TInput">The type of parameter accepted by the functions.</typeparam>
    /// <typeparam name="TOutput">The type of result returned by the functions.</typeparam>
    /// <param name="message">A message mapped to functions.</param>
    /// <returns>A sequence of all functions associated with <c>message</c>.</returns>
    public IEnumerable<Func<TInput, TOutput>> GetFunctions<TInput, TOutput>(MediatorMessage message)
        => CollectCallbacks<Func<TInput, TOutput>>(message);

    /// <summary>
    /// Removes a previously mapped message and callback.
    /// </summary>
    /// <param name="message">The message associated with the callback.</param>
    /// <param name="callback">The callback associated with the function.</param>
    public void RemoveMapping(MediatorMessage message, MulticastDelegate callback)
    {   // We wrap the callback being removed in order to take advantage of the weak callback's equality operations, which will
        // help us locate it in the list of callbacks for the associated message.
        var weakCallback = new WeakCallback(callback);
        IList<WeakCallback> callbacksForMessage = GetCallbacksForMessage(message);

        callbacksForMessage.Remove(weakCallback);
    }

    /// <summary>
    /// Removes the callback mappings associated with the provided message.
    /// </summary>
    /// <param name="message">The message to remove all mappings for.</param>
    public void RemoveMappings(MediatorMessage message)
        => _mappings.Remove(message);

    /// <summary>
    /// Removes all message to callback mappings.
    /// </summary>
        public void RemoveMappings()
            => _mappings.Clear();

    private static void EnsureCompatibleCallback(MediatorMessage message, Type callbackType)
    {
        if (message.CallbackType == null || callbackType == message.CallbackType)
            return;

        if (!message.CallbackType.IsA(callbackType))
        {
            throw new ArgumentException(Strings.InvalidMessageCallbackType.InvariantFormat(
                                         callbackType,
                                         message.Name,
                                         message.CallbackType));}
    }
    
    private List<TCallback> CollectCallbacks<TCallback>(MediatorMessage message)
        where TCallback : class
    {
        EnsureCompatibleCallback(message, typeof(TCallback));

        var collectedCallbacks = new List<TCallback>();
        IList<WeakCallback> callbacksForMessage = GetCallbacksForMessage(message);

        // Move from the back so dead reference removals don't interfere with our iteration.
        for (int i = callbacksForMessage.Count - 1; i > -1; --i) 
        {
            WeakCallback weakCallback = callbacksForMessage[i];

            if (!weakCallback.IsAlive)
                callbacksForMessage.RemoveAt(i);
            else
            {
                if (weakCallback.Target is TCallback callback)
                    collectedCallbacks.Add(callback);
            }
        }

        return collectedCallbacks;
    }
    
    private IList<WeakCallback> GetCallbacksForMessage(MediatorMessage message)
    {
        if (!_mappings.TryGetValue(message, out IList<WeakCallback>? callbacksForMessage))
        {
            callbacksForMessage = new List<WeakCallback>();
            _mappings[message] = callbacksForMessage;
        }

        return callbacksForMessage;
    }
}
