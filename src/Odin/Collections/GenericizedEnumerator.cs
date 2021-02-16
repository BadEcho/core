//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace BadEcho.Odin.Collections
{
    /// <summary>
    /// Provides an enumerator that genericizes the non-generic enumerator of a provided <see cref="IEnumerable"/>
    /// object.
    /// </summary>
    /// <typeparam name="T">The type of elements being enumerated.</typeparam>
    /// <remarks>
    /// The purpose of this class is to allow for an older collection exposing only an <see cref="IEnumerator"/> to be
    /// compatible with an interface that expects the more modern <see cref="IEnumerator{T}"/> variant. Naturally, use
    /// of this class should be avoided if the collection in question already has the means to provide an
    /// <see cref="IEnumerator{T}"/> implemented enumerator.
    /// </remarks>
    public sealed class GenericizedEnumerator<T> : Enumerator, IEnumerator<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericizedEnumerator{T}"/> class.
        /// </summary>
        /// <inheritdoc/>
        public GenericizedEnumerator(IEnumerable enumerable) 
            : base(enumerable)
        { }

        /// <summary>
        /// Gets the element in the collection in its strongly-typed form at the current position
        /// of the enumerator.
        /// </summary>
        public new T Current
            // Interestingly, Microsoft makes use of a null-forgiving operator in what seems to
            // be a near majority of their implementations of this IEnumerator property.
            => (T) base.Current!;

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Even though the <see cref="IEnumerator"/> interface does not implement <see cref="IDisposable"/> (in contrast
        /// to the generic <see cref="IEnumerator{T}"/> version), if an <see cref="IEnumerator"/> is being used within
        /// a foreach loop, the runtime will actually manually check to see if it implements <see cref="IDisposable"/>
        /// anyway (and then call it, of course).
        /// </para>
        /// <para>
        /// We can chalk up this seemingly odd (at first) behavior to a reluctance to break existing contracts in the wild
        /// coupled with poor planning on Microsoft's part. Regardless, because we're essentially wrapping the
        /// <see cref="IEnumerator"/> in our own, we will need to perform this manual check on our own.
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            IDisposable? disposable = InnerEnumerator as IDisposable;

            disposable?.Dispose();
        }
    }
}
