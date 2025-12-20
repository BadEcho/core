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
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionPropertyChangePublisher{T}"/> class.
    /// </summary>
    /// <param name="collection">A source for changes in the collection.</param>
    public CollectionPropertyChangePublisher(INotifyCollectionChanged collection)
    {
        Require.NotNull(collection, nameof(collection));

        collection.CollectionChanged += HandleCollectionChanged;
    }

    /// <summary>
    /// Occurs when there's a change in the collection's composition.
    /// </summary>
    public event EventHandler<CollectionChangedEventArgs>? CollectionChanged;

    /// <summary>
    /// Occurs when there's a change in a property value of one of the collection's items.
    /// </summary>
    public event EventHandler<PropertyChangedEventArgs>? ItemChanged;

    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // These collections are null when they are considered invalid for whatever reason.
        if (e.NewItems != null)
        {
            foreach (INotifyPropertyChanged newItem in e.NewItems)
            {
                newItem.PropertyChanged += HandleItemChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (INotifyPropertyChanged oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= HandleItemChanged;
            }
        }

        var changedArgs = new CollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems);

        CollectionChanged?.Invoke(sender, changedArgs);
    }

    private void HandleItemChanged(object? sender, PropertyChangedEventArgs e) 
        => ItemChanged?.Invoke(sender, e);
}