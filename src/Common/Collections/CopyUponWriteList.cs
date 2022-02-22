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

using System.Collections;

namespace BadEcho.Collections;

/// <summary>
/// Provides a thread-safe <see cref="ArrayList"/> that performs a copy of itself upon each write to support consistency,
/// and is used for various sensitive operations that require such a collection.
/// </summary>
internal class CopyUponWriteList
{
    private ArrayList? _readonlyWrapper;

    /// <summary>
    /// Gets a read-only wrapper of the list.
    /// </summary>
    protected ArrayList ReadOnlyInnerList
    {
        get
        {
            ArrayList readOnlyInnerList;

            lock (InnerListLock)
            {
                _readonlyWrapper ??= ArrayList.ReadOnly(InnerList);

                readOnlyInnerList = _readonlyWrapper;
            }

            return readOnlyInnerList;
        }
    }

    /// <summary>
    /// Gets the actual inner list which lacks any copy-on-write protection.
    /// </summary>
    /// <remarks>
    /// The methods that modify the <see cref="CopyUponWriteList"/> are responsible for checking this and copying it before
    /// modifying it, as well as clearing it when necessary.
    /// </remarks>
    protected ArrayList InnerList
    { get; private set; } = new();

    /// <summary>
    /// Gets the synchronization object used to access the <see cref="InnerList"/>.
    /// </summary>
    protected object InnerListLock
    { get; } = new();

    /// <summary>
    /// Performs a copy and then adds the object to the actual list.
    /// </summary>
    /// <param name="value">The object to add.</param>
    protected void InnerAdd(object value)
    {
        PerformCopyUponWrite();

        InnerList.Add(value);
    }

    /// <summary>
    /// Performs a copy and then inserts the object into the list.
    /// </summary>
    /// <param name="index">THe index to insert the object at.</param>
    /// <param name="value">The item to insert.</param>
    protected void InnerInsert(int index, object value)
    {
        PerformCopyUponWrite();

        InnerList.Insert(index, value);
    }

    /// <summary>
    /// Performs a copy of the list if the given index is valid and then removes the object found at said index.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>True if an item at <c>index</c> is removed; otherwise, false.</returns>
    protected bool RemoveAt(int index)
    {
        if (index < 0 || index >= InnerList.Count)
            return false;

        PerformCopyUponWrite();

        InnerList.RemoveAt(index);

        return true;
    }

    /// <summary>
    /// Executes the copy-upon-write mechanism of this collection.
    /// </summary>
    protected void PerformCopyUponWrite()
    {
        if (_readonlyWrapper == null)
            return;

        InnerList = (ArrayList) InnerList.Clone();

        _readonlyWrapper = null;
    }
}