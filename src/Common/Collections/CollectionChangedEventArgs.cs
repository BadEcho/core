// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;

namespace BadEcho.Collections;

/// <summary>
/// Provides data for a change in a collection's composition as well making up for several shortcomings of
/// <see cref="NotifyCollectionChangedEventArgs"/>.
/// </summary>
public sealed class CollectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="action">The action that caused the event.</param>
    /// <param name="newItems">The list of new items involved in the change.</param>
    /// <param name="oldItems">The list of items affected by a replacement, removal, or movement of members within a collection.</param>
    /// <remarks>
    /// Unlike <see cref="NotifyCollectionChangedEventArgs"/>, which is way too restrictive about which actions are allowed in each of
    /// its constructor overloads, this type allows new and old item data to be provided regardless of the action type.
    /// This ensures that all items are properly set and cleaned up regardless of how they're added or removed.
    /// </remarks>
    public CollectionChangedEventArgs(NotifyCollectionChangedAction action, IList? newItems, IList? oldItems)
        : this(action)
    {
        if (newItems != null)
            NewItems = newItems;

        if (oldItems != null)
            OldItems = oldItems;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="action">The action that caused the event.</param>
    private CollectionChangedEventArgs(NotifyCollectionChangedAction action) 
        => Action = action;

    /// <summary>
    /// Gets the action that caused the event.
    /// </summary>
    public NotifyCollectionChangedAction Action
    { get; }

    /// <summary>
    /// Gets the list of new items involved in the change.
    /// </summary>
    public IList NewItems
    { get; } = new ArrayList();

    /// <summary>
    /// Gets the list of items affected by a replacement, removal, or movement of members within a collection.
    /// </summary>
    public IList OldItems
    { get; } = new ArrayList();
}