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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Collections;

/// <summary>
/// Provides a thread-safe collection of keys paired with lazy values that can be accessed by multiple threads concurrently.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values to be initialized lazily in the dictionary.</typeparam>
public sealed class LazyConcurrentDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _dictionary;
    private readonly LazyThreadSafetyMode _lazyMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="concurrencyLevel">
    /// The estimated number of threads that will update the <see cref="LazyConcurrentDictionary{TKey,TValue}"/> concurrently.
    /// </param>
    /// <param name="capacity">
    /// The initial number of elements that the <see cref="LazyConcurrentDictionary{TKey,TValue}"/> can contain.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(int concurrencyLevel,
                                    int capacity,
                                    IEqualityComparer<TKey> comparer,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(concurrencyLevel, capacity, comparer), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(IEqualityComparer<TKey> comparer,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(comparer), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="concurrencyLevel">
    /// The estimated number of threads that will update the <see cref="LazyConcurrentDictionary{TKey,TValue}"/> concurrently.
    /// </param>
    /// <param name="capacity">
    /// The initial number of elements that the <see cref="LazyConcurrentDictionary{TKey,TValue}"/> can contain.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(int concurrencyLevel,
                                    int capacity,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(concurrencyLevel, capacity), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="concurrencyLevel">
    /// The estimated number of threads that will update the <see cref="LazyConcurrentDictionary{TKey,TValue}"/> concurrently.
    /// </param>
    /// <param name="collection">
    /// A collection whose elements are copied to the new <see cref="LazyConcurrentDictionary{TKey, TValue}"/>.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(int concurrencyLevel,
                                    IEnumerable<KeyValuePair<TKey, Lazy<TValue>>> collection,
                                    IEqualityComparer<TKey> comparer,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(concurrencyLevel, collection, comparer), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">
    /// A collection whose elements are copied to the new <see cref="LazyConcurrentDictionary{TKey, TValue}"/>.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(IEnumerable<KeyValuePair<TKey, Lazy<TValue>>> collection,
                                    IEqualityComparer<TKey> comparer,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(collection, comparer), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">
    /// A collection whose elements are copied to the new <see cref="LazyConcurrentDictionary{TKey, TValue}"/>.
    /// </param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(IEnumerable<KeyValuePair<TKey, Lazy<TValue>>> collection,
                                    LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(collection), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    public LazyConcurrentDictionary(LazyThreadSafetyMode lazyMode)
        : this(new ConcurrentDictionary<TKey, Lazy<TValue>>(), lazyMode)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="dictionary">The inner concurrent dictionary.</param>
    /// <param name="lazyMode">
    /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
    /// </param>
    private LazyConcurrentDictionary(ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, LazyThreadSafetyMode lazyMode)
    {
        _dictionary = dictionary;
        _lazyMode = lazyMode;
    }

    /// <inheritdoc/>
    public int Count
        => _dictionary.Count;

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys
        => _dictionary.Select(pair => pair.Key);

    /// <inheritdoc/>
    /// <remarks>Each value is created, if it has not been already, as the sequence is enumerated.</remarks>
    public IEnumerable<TValue> Values
        => _dictionary.Select(pair => pair.Value.Value);

    /// <inheritdoc/>
    public TValue this[TKey key]
        => _dictionary[key].Value;

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
        => _dictionary.ContainsKey(key);

    /// <summary>
    /// Attempts to get the value associated with the specified key from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>,
    /// creating the value if it has not been already.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the object from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> that
    /// has the specified key, or the default value of the type if the operation failed.
    /// </param>
    /// <returns>
    /// True if <c>key</c> was found in the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>; otherwise, false.
    /// </returns>
    /// <remarks>Use <see cref="TryGetLazy"/> to look up a value without creating it.</remarks>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_dictionary.TryGetValue(key, out Lazy<TValue>? lazyValue))
        {
            value = lazyValue.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get the <see cref="Lazy{TValue}"/> wrapped value associated with the specified key from the
    /// <see cref="LazyConcurrentDictionary{TKey, TValue}"/> without creating the value.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="lazyValue">
    /// When this method returns, contains the <see cref="Lazy{TValue}"/> wrapped value that has the specified key, or null if
    /// the operation failed.
    /// </param>
    /// <returns>
    /// True if <c>key</c> was found in the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>; otherwise, false.
    /// </returns>
    public bool TryGetLazy(TKey key, [NotNullWhen(true)] out Lazy<TValue>? lazyValue)
        => _dictionary.TryGetValue(key, out lazyValue);

    /// <summary>
    /// Adds a key/value pair to the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> by using the specified
    /// function wrapped in a <see cref="Lazy{TValue}"/> instance if the key does not already exist. Returns the
    /// new value, or the existing value if the key exists.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="valueFactory">
    /// The function that will be wrapped in a <see cref="Lazy{TValue}"/> instance and used to generate a value for
    /// the key.
    /// </param>
    /// <returns>
    /// The <see cref="Lazy{TValue}"/> wrapped value for the key. This will be either the existing value for the key
    /// if the key is already in the dictionary, or the new value if the key was not in the dictionary.
    /// </returns>
    public Lazy<TValue> GetOrAdd(TKey key, Func<TValue> valueFactory)
    {
        Require.NotNull(valueFactory, nameof(valueFactory));

        return _dictionary.GetOrAdd(key,
                                    static (_, state) => new Lazy<TValue>(state.ValueFactory, state.LazyMode),
                                    (ValueFactory: valueFactory, LazyMode: _lazyMode));
    }

    /// <summary>
    /// Adds a key/<see cref="Lazy{TValue}"/> pair to the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> by using the
    /// specified function if the key does not already exist. Returns the new value, or the existing value if the key exists.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="lazyFactory">The function used to generate the <see cref="Lazy{TValue}"/> wrapped value for the key.</param>
    /// <returns>
    /// The <see cref="Lazy{TValue}"/> wrapped value for the key. This will be either the existing value for the key
    /// if the key is already in the dictionary, or the new value if the key was not in the dictionary.
    /// </returns>
    /// <remarks>
    /// <c>lazyFactory</c> may run more than once if multiple threads add the same key concurrently, but only one of the
    /// resulting <see cref="Lazy{TValue}"/> instances is stored and returned to all callers.
    /// </remarks>
    public Lazy<TValue> GetOrAdd(TKey key, Func<TKey, Lazy<TValue>> lazyFactory)
        => _dictionary.GetOrAdd(key, lazyFactory);

    /// <summary>
    /// Attempts to remove the value that has the specified key from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>True if the element was removed successfully; otherwise, false.</returns>
    public bool TryRemove(TKey key)
        => _dictionary.TryRemove(key, out _);

    /// <summary>
    /// Removes all keys and values from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>.
    /// </summary>
    public void Clear()
        => _dictionary.Clear();

    /// <inheritdoc/>
    /// <remarks>Each value is created, if it has not been already, when it is read from the enumerator.</remarks>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => new LazyValueEnumerator(_dictionary.GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Provides an enumerator that creates values only when they are read.
    /// </summary>
    private sealed class LazyValueEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly IEnumerator<KeyValuePair<TKey, Lazy<TValue>>> _innerEnumerator;

        public LazyValueEnumerator(IEnumerator<KeyValuePair<TKey, Lazy<TValue>>> innerEnumerator)
            => _innerEnumerator = innerEnumerator;

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var (key, lazyValue) = _innerEnumerator.Current;

                return new KeyValuePair<TKey, TValue>(key, lazyValue.Value);
            }
        }

        object IEnumerator.Current
            => Current;

        public bool MoveNext()
            => _innerEnumerator.MoveNext();

        public void Reset()
            => _innerEnumerator.Reset();

        public void Dispose()
            => _innerEnumerator.Dispose();
    }
}
