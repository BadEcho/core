//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;
using BadEcho.Presentation.Extensions;

namespace BadEcho.Presentation;

/// <summary>
/// Provides an observable collection that ensures all operations meant to change the collection are committed as single,
/// solitary actions, executed in a sufficiently thread-safe context.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
/// <remarks>
/// <para>
/// Certain types of collection-related operations can result in an unwanted plethora of change notification events being fired
/// off, causing an excessive amount of updating to occur with bound UI elements. This can result in a rather large and noticeable
/// drop in the performance of a user interface.
/// </para>
/// <para>
/// An example of one of these types of operation is the "simple" act of sorting: changing the order of elements in a bound observable
/// collection will result in around as many change notifications as would be characterized by the sorting algorithm's complexity. For
/// large collections of thousands of items, this can easily result in thousands of updates to the UI; something not conducive to the idea
/// of an application running smoothly. 
/// </para>
/// <para>
/// Using this collection will ensure operations that would normally result in a large number of individual change notifications being
/// fired only result in a single change notification being fired in order to update bound UI elements to the final new state of the
/// collection. This type also helps ensure that any changes to the collection are synchronized such that only a single change occurs
/// at any given time on the correct dispatcher without any unnecessary blocking.
/// </para>
/// <para>
/// Simply put, instances of this collection type can be manipulated from a non-UI thread without fear of incurring a cross-thread violation
/// exception.
/// </para>
/// </remarks>
public sealed class AtomicObservableCollection<T> : ObservableCollection<T>, IHandlerBypassable
{
    private readonly object _dispatcherLock = new();

    private Dispatcher? _dispatcher = Dispatcher.FromThread(Thread.CurrentThread);

    /// <summary>
    /// Initializes a new instance of the <see cref="AtomicObservableCollection{T}"/> class.
    /// </summary>
    public AtomicObservableCollection()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtomicObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher that communications from this collection should be sent through.</param>
    public AtomicObservableCollection(Dispatcher? dispatcher)
        : this(Enumerable.Empty<T>(), dispatcher)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtomicObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="items">A sequence of items to copy to the new <see cref="AtomicObservableCollection{T}"/>.</param>
    /// <param name="dispatcher">The dispatcher that communications from this collection should be sent through.</param>
    public AtomicObservableCollection(IEnumerable<T> items, Dispatcher? dispatcher)
        : this (items)
    { 
        if (null != dispatcher)
            _dispatcher = dispatcher;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtomicObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="items">A sequence of items to copy to the new <see cref="AtomicObservableCollection{T}"/>.</param>
    public AtomicObservableCollection(IEnumerable<T> items) 
        => AddRange(items);

    /// <summary>
    /// Gets a value indicating if collection modification operations must be invoked through the dispatcher.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_dispatcher))]
    private bool IsDispatcherRequired
        => null != _dispatcher && Thread.CurrentThread != _dispatcher.Thread;

    /// <summary>
    /// Adds the sequence of items to the collection, sending a single collection change notification once all of the items
    /// have been inserted.
    /// </summary>
    /// <param name="items">A sequence of items to add.</param>
    public void AddRange(IEnumerable<T> items)
        => AddRange(items, true);

    /// <summary>
    /// Adds the sequence of items to the collection.
    /// </summary>
    /// <param name="items">A sequence of items to add.</param>
    /// <param name="notifyAfter">
    /// Value indicating if a collection change notification should be sent once the batch of insertions has been completed.
    /// </param>
    public void AddRange(IEnumerable<T> items, bool notifyAfter)
    {
        Require.NotNull(items, nameof(items));

        List<T> itemsList = items.ToList();

        SynchronizeOperation(AddOperation);

        void AddOperation()
        {
            int newItems = AddSilently(itemsList);

            if (!notifyAfter) 
                return;

            if (newItems > 0)
                NotifyReset();
        }
    }

    /// <summary>
    /// Removes the sequence of items from the collection, sending a single collection change notification once all of the items
    /// have been removed.
    /// </summary>
    /// <param name="items">A sequence of items to remove.</param>
    public void RemoveRange(IEnumerable<T> items)
        => RemoveRange(items, true);

    /// <summary>
    /// Removes the sequence of items from the collection.
    /// </summary>
    /// <param name="items">A sequence of items to remove.</param>
    /// <param name="notifyAfter">
    /// Value indicating if a collection change notification should be sent out once the batch of removals has been completed.
    /// </param>
    public void RemoveRange(IEnumerable<T> items, bool notifyAfter)
    {
        Require.NotNull(items, nameof(items));

        // Only you can prevent modified collection enumerations!
        List<T> itemsList = items.ToList();

        SynchronizeOperation(RemoveOperation);

        void RemoveOperation()
        {
            int removedItems = RemoveSilently(itemsList);

            if (!notifyAfter)
                return;

            if (removedItems > 0)
                NotifyReset();
        }
    }

    /// <summary>
    /// Sorts the contents of the observable collection in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of key returned by the selector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    public void OrderBy<TKey>(Func<T, TKey> keySelector)
        => OrderBy(keySelector, Comparer<TKey>.Default);

    /// <summary>
    /// Sorts the contents of the observable collection in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of key returned by the selector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{TKey}"/> to compare keys.</param>
    public void OrderBy<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
        SynchronizeOperation(OrderByOperation);

        void OrderByOperation()
        {
            List<T> sortedItems = Items.OrderBy(keySelector, comparer).ToList();

            CommitSort(sortedItems);
        }
    }

    /// <summary>
    /// Sorts the contents of the observable collection in descending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of key returned by the selector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    public void OrderByDescending<TKey>(Func<T, TKey> keySelector)
        => OrderByDescending(keySelector, Comparer<TKey>.Default);

    /// <summary>
    /// Sorts the contents of the observable collection in descending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of key returned by the selector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{TKey}"/> to compare keys.</param>
    public void OrderByDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
        SynchronizeOperation(OrderByDescendingOperation);

        void OrderByDescendingOperation()
        {
            List<T> sortedItems = Items.OrderByDescending(keySelector, comparer).ToList();

            CommitSort(sortedItems);
        }
    }

    /// <summary>
    /// Changes the dispatcher currently in use by the collection to the provided one.
    /// </summary>
    /// <param name="dispatcher">The dispatcher that the collection should broadcast events on.</param>
    internal void ChangeDispatcher(Dispatcher dispatcher)
    {
        lock (_dispatcherLock)
        {
            _dispatcher = dispatcher;
        }
    }

    /// <inheritdoc/>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!this.IsHandlingBypassed())
            base.OnCollectionChanged(e);
    }

    /// <inheritdoc/>
    protected override void InsertItem(int index, T item)
    {
        Action operation
            = !IsDispatcherRequired
                ? () => BoundedInsertItem(index, item)
                : () => _dispatcher.Invoke(() => BoundedInsertItem(index, item), DispatcherPriority.Send);

        SynchronizeOperation(operation);
    }

    /// <inheritdoc/>
    protected override void ClearItems()
    {
        Action operation
            = !IsDispatcherRequired
                ? base.ClearItems
                : () => _dispatcher.Invoke(base.ClearItems, DispatcherPriority.Send);

        SynchronizeOperation(operation);
    }

    /// <inheritdoc/>
    protected override void MoveItem(int oldIndex, int newIndex)
    {
        Action operation
            = !IsDispatcherRequired
                ? () => BoundedMoveItem(oldIndex, newIndex)
                : () => _dispatcher.Invoke(() => BoundedMoveItem(oldIndex, newIndex), DispatcherPriority.Send);

        SynchronizeOperation(operation);
    }

    /// <inheritdoc/>
    protected override void RemoveItem(int index)
    {
        Action operation
            = !IsDispatcherRequired
                ? () => BoundedRemoveItem(index)
                : () => _dispatcher.Invoke(() => BoundedRemoveItem(index), DispatcherPriority.Send);

        SynchronizeOperation(operation);
    }

    /// <inheritdoc/>
    protected override void SetItem(int index, T item)
    {
        Action operation
            = !IsDispatcherRequired
                ? () => BoundedSetItem(index, item)
                : () => _dispatcher.Invoke(() => BoundedSetItem(index, item), DispatcherPriority.Send);

        SynchronizeOperation(operation);
    }

    private int AddSilently(IEnumerable<T> items)
    {
        int newItems = 0;

        this.BypassHandlers(() =>
                            {
                                foreach (T item in items)
                                {
                                    BoundedInsertItem(Count + newItems, item);
                                    newItems++;
                                }
                            });

        return newItems;
    }

    private int RemoveSilently(IEnumerable<T> items)
    {
        int removedItems = 0;

        this.BypassHandlers(() => removedItems = items.Count(item => Items.Remove(item)));

        return removedItems;
    }

    private void CommitSort(IEnumerable<T> sortedItems)
    {
        // Apparently base virtual method calls in lambda expressions no longer produces unverifiable code.
        this.BypassHandlers(Clear);

        AddSilently(sortedItems);

        NotifyReset();
    }

    private void NotifyReset()
    {
        if (!IsDispatcherRequired)
            OnCollectionReset();
        else
            _dispatcher.Invoke(OnCollectionReset, DispatcherPriority.Send);

        void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    private void SynchronizeOperation(Action operation)
    {
        if (Dispatcher.FromThread(Thread.CurrentThread) != null)
        {
            Trampoline.Execute(() => DispatcherOperationRunner(operation));
        }
        else
        {
            lock (_dispatcherLock)
            {
                operation();
            }
        }

        Bounce DispatcherOperationRunner(Action dispatcherOperation)
        {
            if (!Monitor.TryEnter(_dispatcherLock))
            {
                Dispatcher.CurrentDispatcher.ProcessMessages();

                return Bounce.Continue();
            }

            dispatcherOperation();

            Monitor.Exit(_dispatcherLock);

            return Bounce.Finish();
        }
    }

    private void BoundedInsertItem(int index, T item)
    {
        index = index <= Count ? index : Count;

        base.InsertItem(index, item);
    }

    private void BoundedMoveItem(int oldIndex, int newIndex)
    {
        if (Count == 0)
            return;

        oldIndex = oldIndex <= Count - 1 ? oldIndex : Count - 1;
        newIndex = newIndex <= Count - 1 ? newIndex : Count - 1;

        base.MoveItem(oldIndex, newIndex);
    }

    private void BoundedRemoveItem(int index)
    {
        if (Count == 0)
            return;

        index = index <= Count - 1 ? index : Count - 1;

        base.RemoveItem(index);
    }

    private void BoundedSetItem(int index, T item)
    {
        if (Count == 0)
            return;

        index = index <= Count - 1 ? index : Count - 1;

        base.SetItem(index, item);
    }
}