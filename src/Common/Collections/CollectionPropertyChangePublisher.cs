// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
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
using System.ComponentModel;

namespace BadEcho.Collections;

/// <summary>
/// Provides a publishing service for events pertaining to <see cref="INotifyCollectionChanged"/> capable collections containing
/// <see cref="INotifyPropertyChanged"/> typed items.
/// </summary>
/// <typeparam name="T">The type of item in the <see cref="INotifyCollectionChanged"/> capable collection.</typeparam>
/// <remarks>
/// This event publishing service marries the two separate notions of changes occurring to a collection's composition
/// (<see cref="INotifyCollectionChanged"/>) and changes occurring to a particular item in a collection
/// (<see cref="INotifyPropertyChanged"/>).
/// </remarks>
public sealed class CollectionPropertyChangePublisher<T>
    where T : INotifyPropertyChanged
{
    private Dictionary<INotifyPropertyChanged, int> _subscriptionCounts = new(ReferenceEqualityComparer.Instance);
    private readonly INotifyCollectionChanged _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionPropertyChangePublisher{T}"/> class.
    /// </summary>
    /// <param name="collection">A source for changes in the collection.</param>
    public CollectionPropertyChangePublisher(INotifyCollectionChanged collection)
    {
        Require.NotNull(collection, nameof(collection));

        collection.CollectionChanged += HandleCollectionChanged;

        _collection = collection;

        Subscribe(_collection as IEnumerable);
    }

    /// <summary>
    /// Occurs when there's a change in the collection's composition.
    /// </summary>
    public event EventHandler<NotifyCollectionChangedEventArgs>? CollectionChanged;

    /// <summary>
    /// Occurs when there's a change in a property value of one of the collection's items.
    /// </summary>
    public event EventHandler<PropertyChangedEventArgs>? ItemChanged;

    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {   // NotifyCollectionChangedEventArgs annoyingly does not provide item information via NewItems/OldItems for Reset actions.
            // Resynchronize with the collection's current contents instead, so that items no longer present are released.
            UnsubscribeAll();
            Subscribe(_collection as IEnumerable);
        }
        else
        {   // Unsubscribe old items before subscribing new ones, so that an item present in both lists (e.g., from a Move action) remains subscribed.
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);
        }

        CollectionChanged?.Invoke(sender, e);
    }

    private void Subscribe(IEnumerable? items)
    {
        if (items == null)
            return;

        foreach (object? item in items)
        {
            if (item is not INotifyPropertyChanged notifier)
                continue;

            if (_subscriptionCounts.TryGetValue(notifier, out int count))
            {
                _subscriptionCounts[notifier] = count + 1;
                continue;
            }

            _subscriptionCounts.Add(notifier, 1);
            notifier.PropertyChanged += HandleItemChanged;
        }
    }

    private void Unsubscribe(IEnumerable? items)
    {
        if (items == null)
            return;

        foreach (object? item in items)
        {
            if (item is not INotifyPropertyChanged notifier || !_subscriptionCounts.TryGetValue(notifier, out int count))
                continue;

            // An item that still occurs elsewhere in the collection stays subscribed.
            if (count > 1)
            {
                _subscriptionCounts[notifier] = count - 1;
                continue;
            }

            _subscriptionCounts.Remove(notifier);
            notifier.PropertyChanged -= HandleItemChanged;
        }
    }

    private void UnsubscribeAll()
    {
        foreach (INotifyPropertyChanged notifier in _subscriptionCounts.Keys)
        {
            notifier.PropertyChanged -= HandleItemChanged;
        }

        _subscriptionCounts.Clear();
    }

    private void HandleItemChanged(object? sender, PropertyChangedEventArgs e) 
        => ItemChanged?.Invoke(sender, e);
}