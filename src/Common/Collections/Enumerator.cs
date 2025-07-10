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
/// Provides a customizable enumerator that supports a simple iteration over a collection.
/// </summary>
/// <remarks>
/// This class leaves the particulars of the enumeration mechanism to the other enumerator provided to it, allowing
/// us to extend the base functionality of the provided enumerator with customizations defined in a derived
/// class. 
/// </remarks>
/// <suppressions>
/// ReSharper disable NotDisposedResource
/// The enumerator is disposed if it implements <see cref="IDisposable"/> by the <see cref="Enumerator{T}"/> class.
/// Other derivations of this type are responsible for replicating this behavior.
/// </suppressions>
public abstract class Enumerator : IEnumerator
{
    private bool _endOfSequence;

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumerator"/> class.
    /// </summary>
    /// <param name="enumerable">A sequence whose functionally independent enumerator will be decorated.</param>
    protected Enumerator(IEnumerable enumerable)
    {
        Require.NotNull(enumerable, nameof(enumerable));
        
        InnerEnumerator = enumerable.GetEnumerator();
    }
        
    /// <inheritdoc/>
    public object? Current
    {
        get
        {
            EnsureReadablePosition();

            return InnerEnumerator.Current;
        }
    }

    /// <summary>
    /// Gets the <see cref="IEnumerator"/> object being enumerated.
    /// </summary>
    protected IEnumerator InnerEnumerator
    { get; }

    /// <summary>
    /// Gets the current position of the enumerator.
    /// </summary>
    protected int Index
    { get; private set; }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        bool success = InnerEnumerator.MoveNext();

        if (success)
            Index++;
        else
            _endOfSequence = true;

        return success;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        InnerEnumerator.Reset();

        Index = 0;
        _endOfSequence = false;
    }

    /// <summary>
    /// Ensures that the enumerator's position is such that the enumerator is currently pointed to an element in the sequence.
    /// </summary>
    protected void EnsureReadablePosition()
    {
        if (Index == 0)
            throw new InvalidOperationException(Strings.EnumerationNotStarted);

        if (_endOfSequence)
            throw new InvalidOperationException(Strings.EnumerationAtEnd);
    }
}