//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Logging;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Collections
{
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
        /// Occurs when a change in either the collection's composition or a property value of one of its items occurs.
        /// </summary>
        public event EventHandler<CollectionPropertyChangedEventArgs>? Changed;

        private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // These collections are null when they are considered invalid for whatever reason.
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged newItem in e.NewItems)
                {
                    newItem.PropertyChanged += HandleItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= HandleItemPropertyChanged;
                }
            }

            var changedArgs =
                new CollectionPropertyChangedEventArgs((CollectionPropertyChangedAction) e.Action, e.NewItems, e.OldItems);

            Changed?.Invoke(this, changedArgs);
        }

        private void HandleItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null)
            {
                Logger.Warning(Strings.BadINotifyPropertyChangedImplementation
                                      .InvariantFormat(typeof(T)));
                return;
            }
            
            var changedArgs = new CollectionPropertyChangedEventArgs(e.PropertyName);

            Changed?.Invoke(this, changedArgs);
        }

    }
}
