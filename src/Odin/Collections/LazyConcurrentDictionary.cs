//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Odin.Collections
{
    /// <summary>
    /// Provides a thread-safe collection of keys paired with lazy values that can be accessed by multiple threads concurrently.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values to be initialized lazily in the dictionary.</typeparam>
    public sealed class LazyConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey,Lazy<TValue>>, IReadOnlyDictionary<TKey,TValue>
        where TKey : notnull
    {
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
            : base(concurrencyLevel, capacity, comparer)
        {
            _lazyMode = lazyMode;
        }

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
            : base(comparer)
        {
            _lazyMode = lazyMode;
        }

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
            : base(concurrencyLevel, capacity)
        { 
            _lazyMode = lazyMode;
        }

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
            : base(concurrencyLevel, collection, comparer)
        {
            _lazyMode = lazyMode;
        }

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
            : base(collection, comparer)
        {
            _lazyMode = lazyMode;
        }

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
            : base(collection)
        {
            _lazyMode = lazyMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="lazyMode">
        /// An enumeration value specifying how <see cref="Lazy{T}"/> instances synchronize access among multiple threads.
        /// </param>
        public LazyConcurrentDictionary(LazyThreadSafetyMode lazyMode)
        {
            _lazyMode = lazyMode;
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] 
            => base[key].Value;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys 
            => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
            => Values.Select(lazyValue => lazyValue.Value).ToList();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            => new Enumerator<KeyValuePair<TKey, TValue>>(
                this,
                element =>
                {
                    var (key, value) = (KeyValuePair<TKey, Lazy<TValue>>) element;

                    return new KeyValuePair<TKey, TValue>(key, value.Value);
                });

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
            => GetOrAdd(key, _ => new Lazy<TValue>(valueFactory, _lazyMode));

        /// <summary>
        /// Attempts to get the value associated with the specified key from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>. 
        /// </summary>
        /// <param name="key">THe key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the object from the <see cref="LazyConcurrentDictionary{TKey, TValue}"/> that
        /// has the specified key, or the default value of the type if the operation failed.
        /// </param>
        /// <returns>
        /// True if <c>key</c> was found in the <see cref="LazyConcurrentDictionary{TKey, TValue}"/>; otherwise, false.
        /// </returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)]out TValue value)
        {
            value = default;
            bool success = TryGetValue(key, out Lazy<TValue>? lazyValue);

            if (success && lazyValue != null)
                value = lazyValue.Value;

            return success;
        }
    }
}