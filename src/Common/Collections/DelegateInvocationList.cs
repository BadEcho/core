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
using BadEcho.Properties;

namespace BadEcho.Collections;

/// <summary>
/// Provides a thread-safe manipulable invocation list for a delegate.
/// </summary>
/// <typeparam name="T">The type of delegate the invocation list is for.</typeparam>
/// <suppressions>
/// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
/// ReSharper disable PossibleUnintendedReferenceComparison
/// </suppressions>
public sealed class DelegateInvocationList<T> : IEnumerable<T>
    where T : class, Delegate
{
    private Tuple<T, Delegate[]>? _delegates;

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Adds the provided delegate's invocation list to the invocation list being maintained.
    /// </summary>
    /// <param name="delegate">The delegate that supplies the invocation list to add.</param>
    /// <remarks>This is the same technique used by the C# compiler for code that is subscribing to an event.</remarks>
    public void Add(T @delegate)
    {
        while (true)
        {
            var oldDelegates = _delegates;

            T? combinedDelegate =
                (T?) Delegate.Combine(oldDelegates?.Item1, @delegate);
            
            Tuple<T, Delegate[]>? newDelegates =
                combinedDelegate != null ? new(combinedDelegate, combinedDelegate.GetInvocationList()) : null;
            
            if (Interlocked.CompareExchange(ref _delegates, newDelegates, oldDelegates) == oldDelegates)
                break;
        }
    }

    /// <summary>
    /// Removes the provided delegate's invocation list from the invocation list being maintained.
    /// </summary>
    /// <param name="delegate">The delegate that supplies the invocation list to remove.</param>
    /// <remarks>This is the same technique used by the C# compiler for code that is unsubscribing from an event.</remarks>
    public void Remove(T @delegate)
    {
        while (true)
        {
            var oldDelegates = _delegates;

            T? delegateWithRemoval =
                (T?) Delegate.Remove(oldDelegates?.Item1, @delegate);
            
            Tuple<T, Delegate[]>? newDelegates =
                delegateWithRemoval != null ? new(delegateWithRemoval, delegateWithRemoval.GetInvocationList()) : null;

            if (Interlocked.CompareExchange(ref _delegates, newDelegates, oldDelegates) == oldDelegates)
                break;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
        => new InvocationListEnumerator(_delegates?.Item2);

    /// <summary>
    /// Provides enumeration capabilities for a <see cref="DelegateInvocationList{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You may have noticed that this enumerator is a struct, and a mutable one at that. An enumerator implemented
    /// as a struct is unorthodox enough; a mutable struct is practically Lovecraftian, and almost always a terrible idea.
    /// </para>
    /// <para>
    /// This enumerator is a struct in lieu of the use of value type enumerators found inside .NET's BCL. The team responsible
    /// for the BCL did research and found that the vast majority of the time, the penalty being received for allocating and
    /// deallocating the enumerator is large enough to warrant use of a value type instead.
    /// </para>
    /// <para>
    /// A detailed explanation can be found at:
    /// http://stackoverflow.com/questions/3168311/why-bcl-collections-use-struct-enumerators-not-classes
    /// </para>
    /// <para>
    /// While the referenced information above may all be true, adopting a policy to always use structs for enumerators in our
    /// own code would be foolhardy, given the ease in which problems can occur. Rather, this particular enumerator exists in order
    /// to be used in very specific contexts for which precedent for its use in said contexts has been firmly established by
    /// Microsoft's own code.
    /// </para>
    /// </remarks>
    private struct InvocationListEnumerator : IEnumerator<T>
    {
        private readonly Delegate[] _delegates;
        private T? _current;
        private int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationListEnumerator"/> class.
        /// </summary>
        /// <param name="delegates">The delegates to enumerate.</param>
        public InvocationListEnumerator(Delegate[]? delegates)
        {
            _delegates = delegates ?? [];
        }

        readonly object IEnumerator.Current 
            => Current;

        /// <inheritdoc/>
        public readonly T Current
        {
            get
            {
                if (_current == null)
                    throw new InvalidOperationException(Strings.InvocationListEnumeratorUndefinedCurrent);

                return _current;
            }
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            _current = _index < _delegates.Length ? (T) _delegates[_index++] : null;

            return _current != null;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _index = 0;
            _current = null;
        }

        /// <inheritdoc/>
        public readonly void Dispose()
        { }
    }
}
