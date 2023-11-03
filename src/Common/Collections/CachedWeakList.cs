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
/// Provides a cached and thread-safe list of weak references that is used for various sensitive operations
/// that require such a collection.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
internal sealed class CachedWeakList<T> : CopyUponWriteList<WeakReference<T>>, IEnumerable<T>
    where T : class
{
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    /// <summary>
    /// Adds a new weak reference to the list.
    /// </summary>
    /// <param name="value">An object to add.</param>
    /// <returns>True if <c>value</c> was added; otherwise, false.</returns>
    public bool Add(T value)
        => Add(value, false);

    /// <summary>
    /// Adds a new weak reference to the list.
    /// </summary>
    /// <param name="value">An object to add.</param>
    /// <param name="skipFind">
    /// Value indicating whether a search that ensures no item already a part of the list gets added again is skipped.
    /// </param>
    /// <returns>True if <c>value</c> was added; otherwise, false.</returns>
    public bool Add(T value, bool skipFind)
    {
        if (skipFind)
        {
            if (InnerList.Count == InnerList.Capacity)
                Purge();
        }

        else if (FindReferenceIndex(value) >= 0)
            return false;

        InnerAdd(new WeakReference<T>(value));

        return true;
    }

    /// <summary>
    /// Inserts a weak reference into the list.
    /// </summary>
    /// <param name="index">The index to insert the reference at.</param>
    /// <param name="value">The object to insert.</param>
    /// <returns>True if <c>value</c> was inserted; otherwise, false.</returns>
    public bool Insert(int index, T value)
    {
        lock (InnerListLock)
        {
            int existingIndex = FindReferenceIndex(value);

            if (existingIndex >= 0)
                return false;

            InnerInsert(index, new WeakReference<T>(value));

            return true;
        }
    }
    
    /// <summary>
    /// Removes an object from the list.
    /// </summary>
    /// <param name="value">The object to remove.</param>
    /// <returns>True if the object was removed; otherwise, false.</returns>
    public bool Remove(T value)
    {
        lock (InnerListLock)
        {
            int index = FindReferenceIndex(value);

            return index >= 0 && RemoveAt(index);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
        => new CurrentWeakListEnumerator(ReadOnlyInnerList);
    
    private int FindReferenceIndex(T value)
    {
        bool deadReferences = true;
        int foundIndex = -1;

        while (deadReferences)
        {
            deadReferences = false;

            var list = InnerList;

            for (int i = 0; i < list.Count; i++)
            {
                var weakReference = (WeakReference<T>?) list[i];

                if (weakReference?.TryGetTarget(out T? target) == true)
                {
                    if (target == value)
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
        var list = InnerList;

        int destinationIndex;
        int count = list.Count;

        for (destinationIndex = 0; destinationIndex < count; ++destinationIndex)
        {
            var weakReference = (WeakReference<T>?) list[destinationIndex];

            if (weakReference?.TryGetTarget(out T? _) == true)
                break;
        }

        if (destinationIndex >= count)
            return;

        PerformCopyUponWrite();

        list = InnerList;

        for (int i = destinationIndex + 1; i < count; ++i)
        {
            var weakReference = (WeakReference<T>?) list[i];

            if (weakReference?.TryGetTarget(out T? _) == true)
                list[destinationIndex++] = list[i];
        }

        if (destinationIndex >= count)
            return;

        list.RemoveRange(destinationIndex, count - destinationIndex);

        int newCapacity = destinationIndex << 1;

        if (newCapacity < list.Capacity)
            list.Capacity = newCapacity;
    }

    /// <summary>
    /// Provides enumeration capabilities for a <see cref="CachedWeakList{T}"/>, checking each item enumerated for
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
    private struct CurrentWeakListEnumerator : IEnumerator<T>
    {
        private readonly IReadOnlyList<WeakReference<T>> _list;

        private int _index;
        private T? _strongReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentWeakListEnumerator"/> class.
        /// </summary>
        /// <param name="list">The list to enumerate.</param>
        public CurrentWeakListEnumerator(IReadOnlyList<WeakReference<T>> list)
        {
            _index = 0;
            _list = list;
            _strongReference = null;
        }

        readonly object IEnumerator.Current
            => Current;

        /// <inheritdoc/>
        public readonly T Current
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
            T? strongReference = null;

            while (_index < _list.Count)
            {
                var weakReference = (WeakReference<T>?) _list[_index++];

                if (weakReference?.TryGetTarget(out strongReference) == true)
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

        /// <inheritdoc/>
        public readonly void Dispose()
        { }
    }
}