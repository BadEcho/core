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

using System.Security.Cryptography;

namespace BadEcho;

/// <summary>
/// Provides a weighted random number value generator.
/// </summary>
/// <typeparam name="T">The type of value generated.</typeparam>
public sealed class WeightedRandom<T>
{
    private readonly List<T> _values = [];

    /// <summary>
    /// Adds a weighted value that may be randomly returned.
    /// </summary>
    /// <param name="value">The particular random value.</param>
    /// <param name="weight">The probability that the provided value may be returned.</param>
    public void AddWeight(T value, int weight) 
        => _values.AddRange(Enumerable.Repeat(value, weight));

    /// <summary>
    /// Gets the next weighted random value in the sequence.
    /// </summary>
    /// <returns>The next <typeparamref name="T"/> weighted value in the sequence.</returns>
    public T? Next()
        => _values.Count == 0 ? default : _values[RandomNumberGenerator.GetInt32(0, _values.Count)];
}