//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

namespace BadEcho.Collections;

/// <summary>
/// Provides a thread-safe list that performs a copy of itself upon each write to support consistency, and is used for
/// various sensitive operations that require such a collection.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class CopyUponWriteList<T>
{
    private IReadOnlyList<T>? _readonlyWrapper;

    /// <summary>
    /// Gets a read-only wrapper of the list.
    /// </summary>
    protected IReadOnlyList<T> ReadOnlyItems
    {
        get
        {
            IReadOnlyList<T> readonlyItems;

            lock (ItemsLock)
            {
                _readonlyWrapper ??= Items.AsReadOnly();

                readonlyItems = _readonlyWrapper;
            }

            return readonlyItems;
        }
    }
    
    /// <summary>
    /// Gets the actual inner list which lacks any copy-on-write protection.
    /// </summary>
    /// <remarks>
    /// The methods that modify the <see cref="CopyUponWriteList{T}"/> are responsible for checking this and copying it before
    /// modifying it, as well as clearing it when necessary.
    /// </remarks>
    internal List<T> Items
    { get; private set; } = [];

    /// <summary>
    /// Gets the synchronization object used to access the <see cref="Items"/>.
    /// </summary>
    protected object ItemsLock
    { get; } = new();

    /// <summary>
    /// Performs a copy and then adds the object to the actual list.
    /// </summary>
    /// <param name="value">The object to add.</param>
    protected void ItemsAdd(T value)
    {
        PerformCopyUponWrite();

        Items.Add(value);
    }

    /// <summary>
    /// Performs a copy and then inserts the object into the list.
    /// </summary>
    /// <param name="index">THe index to insert the object at.</param>
    /// <param name="value">The item to insert.</param>
    protected void ItemsInsert(int index, T value)
    {
        PerformCopyUponWrite();

        Items.Insert(index, value);
    }

    /// <summary>
    /// Performs a copy of the list if the given index is valid and then removes the object found at said index.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>True if an item at <c>index</c> is removed; otherwise, false.</returns>
    protected bool RemoveAt(int index)
    {
        if (index < 0 || index >= Items.Count)
            return false;

        PerformCopyUponWrite();

        Items.RemoveAt(index);

        return true;
    }

    /// <summary>
    /// Executes the copy-upon-write mechanism of this collection.
    /// </summary>
    protected void PerformCopyUponWrite()
    {
        if (_readonlyWrapper == null)
            return;

        Items = [..Items];

        _readonlyWrapper = null;
    }
}