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

namespace BadEcho.Odin.Collections;

/// <summary>
/// Provides an enumerator that shapes the output of a provided <see cref="IEnumerable"/> object's enumerator.
/// </summary>
/// <typeparam name="T">The type of elements ultimately being enumerated.</typeparam>
/// <remarks>
/// This class allows for the typed enumeration of elements returned by the enumerator of the provided <see cref="IEnumerable"/>
/// by converting the elements to <typeparamref name="T"/> either directly (default) or with the aid of an intermediate conversion
/// function.
/// </remarks>
public sealed class Enumerator<T> : Enumerator, IEnumerator<T>
{
    private readonly Func<object, T> _elementConverter = current => (T)current;

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumerator{T}"/> class.
    /// </summary>
    /// <param name="enumerable">
    /// A sequence containing elements directly convertible to <typeparamref name="T"/> whose functionally independent enumerator
    /// will be decorated.
    /// </param>
    /// <remarks>
    /// Use this overload if the elements of <c>enumerable</c> are directly convertible to <typeparamref name="T"/>, either due
    /// to the type of <c>enumerable</c> not having a generic <see cref="IEnumerable{T}"/> variant, or because of inherent reference
    /// or value type conversions existing between <typeparamref name="T"/> and the type of element returned by <c>enumerable</c>.
    /// </remarks>
    public Enumerator(IEnumerable enumerable) 
        : base(enumerable)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumerator{T}"/> class.
    /// </summary>
    /// <param name="enumerable">A sequence whose functionally independent enumerator will be decorated.</param>
    /// <param name="elementConverter">Function that converts an element from the provided sequence to <typeparamref name="T"/>.</param>
    public Enumerator(IEnumerable enumerable, Func<object, T> elementConverter)
        : base(enumerable)
    {
        Require.NotNull(elementConverter, nameof(elementConverter));

        _elementConverter = elementConverter;
    }

    /// <summary>
    /// Gets the element in the collection in its strongly-typed form at the current position
    /// of the enumerator.
    /// </summary>
    public new T Current
        // Interestingly, Microsoft makes use of a null-forgiving operator in what seems to
        // be a near majority of their implementations of this IEnumerator property.
        => _elementConverter(base.Current!);

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