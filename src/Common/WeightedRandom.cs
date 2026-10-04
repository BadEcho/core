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

namespace BadEcho;

/// <summary>
/// Provides a weighted random number value generator.
/// </summary>
/// <typeparam name="T">The type of value generated.</typeparam>
public sealed class WeightedRandom<T>
{
    private readonly List<T> _values = [];
    private readonly List<long> _cumulativeWeights = [];
    private readonly Random _random;

    private long _totalWeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedRandom{T}"/> class.
    /// </summary>
    public WeightedRandom()
    {
        _random = Random.Shared;
    }

    /// <summary>
    /// Adds a weighted value that may be randomly returned.
    /// </summary>
    /// <param name="value">The particular random value.</param>
    /// <param name="weight">The probability that the provided value may be returned.</param>
    public void AddWeight(T value, int weight)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight);

        if (weight == 0)
            return;

        _totalWeight += weight;

        _values.Add(value);
        _cumulativeWeights.Add(_totalWeight);
    }

    /// <summary>
    /// Gets the next weighted random value in the sequence.
    /// </summary>
    /// <returns>The next <typeparamref name="T"/> weighted value in the sequence.</returns>
    public T? Next()
    {
        if (_totalWeight == 0)
            return default;

        long roll = _random.NextInt64(_totalWeight);
        int index = _cumulativeWeights.BinarySearch(roll);

        index = index >= 0 ? index + 1 : ~index;

        return _values[index];
    }
}