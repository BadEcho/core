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

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to <see cref="IDictionary{TKey,TValue}"/> objects.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets or adds the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key parameter.</typeparam>
    /// <typeparam name="TValue">The type of the value parameter.</typeparam>
    /// <param name="dictionary">The dictionary containing the specified key.</param>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="valueProvider">The value provider function, used if no value is associated with the key.</param>
    /// <returns>
    /// The value associated with the specified key, if the key is found; otherwise, it retrieves the value from the value \
    /// provider function, adds it to the dictionary and then returns it.
    /// </returns>
    public static TValue GetValueOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueProvider)
    {
        Require.NotNull(dictionary, nameof(dictionary));

        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            Require.NotNull(valueProvider, nameof(valueProvider));

            value = valueProvider(key);
            dictionary.Add(key, value);
        }

        return value;
    }
}
