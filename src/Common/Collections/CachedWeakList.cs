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

using System.Collections;
using BadEcho.Properties;

namespace BadEcho.Collections;

/// <summary>
/// Provides a cached thread-safe <see cref="ArrayList"/> of weak references that is used for various sensitive operations
/// that require such a collection.
/// </summary>
internal sealed class CachedWeakList : CopyUponWriteList, IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Removes an object from the list.
    /// </summary>
    /// <param name="value">The object to remove.</param>
    /// <returns>True if the object was removed; otherwise, false.</returns>
    public bool Remove(object value)
    {
        lock (InnerListLock)
        {
            int index = FindReferenceIndex(value);

            return index >= 0 && RemoveAt(index);
        }
    }

    /// <summary>
    /// Adds a new weak reference to the list.
    /// </summary>
    /// <param name="value">An object to add.</param>
    /// <returns>True if <c>value</c> was added; otherwise, false.</returns>
    public bool Add(object value)
        => Add(value, false);

    /// <summary>
    /// Adds a new weak reference to the list.
    /// </summary>
    /// <param name="value">An object to add.</param>
    /// <param name="skipFind">
    /// Value indicating whether a search that ensures no item already a part of the list gets added again is skipped.
    /// </param>
    /// <returns>True if <c>value</c> was added; otherwise, false.</returns>
    public bool Add(object value, bool skipFind)
    {
        if (skipFind)
        {
            if (InnerList.Count == InnerList.Capacity)
                Purge();
        }

        else if (FindReferenceIndex(value) >= 0)
            return false;

        InnerAdd(new WeakReference(value));

        return true;
    }

    /// <summary>
    /// Inserts a weak reference into the list.
    /// </summary>
    /// <param name="index">The index to insert the reference at.</param>
    /// <param name="value">The object to insert.</param>
    /// <returns>True if <c>value</c> was inserted; otherwise, false.</returns>
    public bool Insert(int index, object value)
    {
        lock (InnerListLock)
        {
            int existingIndex = FindReferenceIndex(value);

            if (existingIndex >= 0)
                return false;

            InnerInsert(index, new WeakReference(value));

            return true;
        }
    }

    private int FindReferenceIndex(object value)
    {
        bool deadReferences = true;
        int foundIndex = -1;

        while (deadReferences)
        {
            deadReferences = false;

            ArrayList list = InnerList;

            for (int i = 0; i < list.Count; i++)
            {
                var weakReference = (WeakReference?) list[i];

                if (weakReference is { IsAlive: true })
                {
                    if (weakReference.Target == value)
                    {
                        foundIndex = i;
                        break;
                    }
                }
                else
                    deadReferences = true;
            }

            if (deadReferences)
                Purge();
        }

        return foundIndex;
    }
        
    private void Purge()
    {
        ArrayList list = InnerList;

        int destinationIndex;
        int count = list.Count;

        for (destinationIndex = 0; destinationIndex < count; ++destinationIndex)
        {
            var weakReference = (WeakReference?) list[destinationIndex];

            if (weakReference is { IsAlive: true })
                break;
        }

        if (destinationIndex >= count)
            return;

        PerformCopyUponWrite();

        list = InnerList;

        for (int i = destinationIndex + 1; i < count; ++i)
        {
            var weakReference = (WeakReference?) list[i];

            if (weakReference is {IsAlive: true})
                list[destinationIndex++] = list[i];
        }

        if (destinationIndex >= count)
            return;

        list.RemoveRange(destinationIndex, count - destinationIndex);

        int newCapacity = destinationIndex << 1;

        if (newCapacity < list.Capacity)
            list.Capacity = newCapacity;
    }

    private CurrentWeakListEnumerator GetEnumerator()
        => new(ReadOnlyInnerList);

    /// <summary>
    /// Provides enumeration capabilities for a <see cref="CachedWeakList"/>, checking each item enumerated for
    /// "aliveness", returning said items as strong references unless they're dead.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You may have noticed that this enumerator is a struct, and a mutable one at that. An enumerator implemented
    /// as a struct is unorthodox enough; a mutable struct is practically Lovecraftian, and almost always a terrible idea.
    /// </para>
    /// <para>
    /// This enumerator is a struct in lieu of the use of value type enumerators found inside .NET's BCL. The team responsible
    /// for the BCL did research and found that the vast majority of the time, the penalty we receiving for allocating and deallocating
    /// the enumerator is large enough to warrant use of a value type instead.
    /// </para>
    /// <para>
    /// A detailed explanation can be found at:
    /// http://stackoverflow.com/questions/3168311/why-bcl-collections-use-struct-enumerators-not-classes
    /// </para>
    /// <para>
    /// While the referenced information above may all be true, adopting a policy to always use structs for enumerators in our
    /// own code would be foolhardy, given the ease in which problems can occur. Rather, this particular enumerator exists in order to be
    /// used in very specific contexts for which precedent for its use in said contexts has been firmly established by Microsoft's own
    /// code.
    /// </para>
    /// </remarks>
    private struct CurrentWeakListEnumerator : IEnumerator
    {
        private readonly ArrayList _list;

        private int _index;
        private object? _strongReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentWeakListEnumerator"/> class.
        /// </summary>
        /// <param name="list">The list to enumerate.</param>
        public CurrentWeakListEnumerator(ArrayList list)
        {
            _index = 0;
            _list = list;
            _strongReference = null;
        }

        object IEnumerator.Current
            => Current;

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        private object Current
        {
            get
            {
                if (_strongReference == null)
                    throw new InvalidOperationException(Strings.WeakListEnumeratorNoReference);

                return _strongReference;
            }
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            object? strongReference = null;

            while (_index < _list.Count)
            {
                var weakReference = (WeakReference?) _list[_index++];

                strongReference = weakReference?.Target;

                if (strongReference != null)
                    break;
            }

            _strongReference = strongReference;

            return strongReference != null;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _index = 0;
            _strongReference = null;
        }
    }
}