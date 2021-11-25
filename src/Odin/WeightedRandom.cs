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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace BadEcho.Odin
{
    /// <summary>
    /// Provides a weighted random number value generator.
    /// </summary>
    /// <typeparam name="T">The type of value generated.</typeparam>
    /// <remarks>
    /// TODO: To be revisited to make thread-safe and not so bare bones.
    /// </remarks>
    public sealed class WeightedRandom<T>
    {
        private readonly List<T> _values = new();

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
            => _values.Count == 0 ? default : _values[RandomNumberGenerator.GetInt32(0, _values.Count - 1)];
    }
}
