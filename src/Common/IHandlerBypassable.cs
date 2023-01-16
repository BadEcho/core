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

using System.Collections.Concurrent;

namespace BadEcho;

/// <summary>
/// Defines a subscriber of events, capable of optionally having the handling of said events bypassed.
/// </summary>
/// <remarks>
/// This is useful for types that, in response to an event firing, need to manipulate the event publisher object in a manner
/// that may result in further firings of the event. By bypassing the handler of said event, we can avoid the inevitable
/// stack overflow caused by infinite recursion. 
/// </remarks>
public interface IHandlerBypassable
{
    private static readonly ConcurrentDictionary<IHandlerBypassable, bool> _HandlersBypassedMap
        = new();

    /// <summary>
    /// Gets a value indicating if event handlers should be bypassed.
    /// </summary>
    internal bool HandlersBypassed
    {
        get => _HandlersBypassedMap.GetOrAdd(this, false);
        set => _HandlersBypassedMap[this] = value;
    }
}

/// <summary>
/// Provides a set of static methods that simplify the use of <see cref="IHandlerBypassable"/> objects.
/// </summary>
public static class HandlerBypassableExtensions
{
    /// <summary>
    /// Executes the provided method while preventing all participating event handlers from reacting to it.
    /// </summary>
    /// <param name="source">An object that supports the bypassing of event handlers.</param>
    /// <param name="action">The method to execute while bypassing all participating event handlers.</param>
    public static void BypassHandlers(this IHandlerBypassable source, Action action)
    {
        Require.NotNull(source, nameof(source));
        Require.NotNull(action, nameof(action));

        source.HandlersBypassed = true;

        action();

        source.HandlersBypassed = false;
    }

    /// <summary>
    /// Gets a value indicating if event handlers should be bypassed.
    /// </summary>
    /// <param name="source">An object that supports the bypassing of event handlers.</param>
    /// <returns>True if <c>source</c> is bypassing handlers; otherwise, false.</returns>
    /// <remarks>
    /// This value returned by this method should be consulted by all implementations of <see cref="IHandlerBypassable"/>
    /// prior to the execution of any consequential code in an event handler.
    /// </remarks>
    public static bool IsHandlingBypassed(this IHandlerBypassable source)
    {
        Require.NotNull(source, nameof(source));

        return source.HandlersBypassed;
    }
}