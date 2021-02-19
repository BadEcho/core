//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;
using BadEcho.Odin.Collections;

namespace BadEcho.Odin.XmlConfiguration
{
    /// <summary>
    /// Provides a configuration element containing a collection of strongly-typed child elements.
    /// </summary>
    /// <typeparam name="TElement">The type of configuration element contained by the collection.</typeparam>
    /// <typeparam name="TKey">The type of key that identifies the elements contained by the collection.</typeparam>
    internal abstract class ConfigurationElementCollection<TElement, TKey> : ConfigurationElementCollection, IEnumerable<TElement>
        where TElement : ConfigurationElement, new()
        where TKey : notnull
    {
        /// <summary>
        /// Gets the configuration element matching the specified key.
        /// </summary>
        /// <param name="key">The key of the element to return.</param>
        /// <returns>The <typeparamref name="TElement"/> identified by <c>key</c>; otherwise, null.</returns>
        public TElement? this[TKey key]
            => (TElement?) BaseGet(key);

        /// <summary>
        /// Adds the provided configuration element to this collection.
        /// </summary>
        /// <param name="element">A configuration element to add.</param>
        public void Add(TElement element) 
            => BaseAdd(element, true);

        /// <summary>
        /// Clears this collection of all configuration elements.
        /// </summary>
        public void Clear() 
            => BaseClear();

        /// <summary>
        /// Checks if this collection contains any element identified byt he provided key.
        /// </summary>
        /// <param name="key">A key that identifies an element.</param>
        /// <returns>True if this contains an item identified by <c>key</c>; otherwise, false.</returns>
        public bool Contains(TKey key) 
            => null != BaseGet(key);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{TElement}"/> that can be used to iterate through the collection.</returns>
        public new IEnumerator<TElement> GetEnumerator() 
            => new GenericizedEnumerator<TElement>(this);

        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement()
            => new TElement();

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element)
        {
            TElement typedElement = (TElement)element;
            
            return GetElementKey(typedElement);
        }

        /// <summary>
        /// Gets the element key for a <typeparamref name="TElement"/> object when overriden in a derived class.
        /// </summary>
        /// <param name="element">The element to return the key for.</param>
        /// <returns>The <typeparamref name="TElement"/> that acts as a key for <c>element</c>.</returns>
        protected abstract TKey GetElementKey(TElement element);
    }
}
