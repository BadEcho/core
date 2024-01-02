//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------


using System.Globalization;

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to type conversion.
/// </summary>
public static class TypeConversionExtensions
{
    /// <summary>
    /// Determines if an instance of this type can be assigned to a variable of another type.
    /// </summary>
    /// <param name="type">The current type.</param>
    /// <param name="otherType">The type to compare with the current type.</param>
    /// <returns>
    /// True if an instance of <c>type</c> can be assigned to a variable of <c>otherType</c>; otherwise, false.
    /// </returns>
    public static bool IsA(this Type type, Type otherType)
    {
        Require.NotNull(otherType, nameof(otherType));
            
        return otherType.IsAssignableFrom(type);
    }

    /// <summary>
    /// Determines if an instance of this type can be assigned to a variable of another type.
    /// </summary>
    /// <typeparam name="T">The type to compare with the current type.</typeparam>
    /// <param name="type">The current type.</param>
    /// <returns>
    /// True if an instance of <c>type</c> can be assigned to a variable of <typeparamref name="T"/>; otherwise, false.
    /// </returns>
    public static bool IsA<T>(this Type type) 
        => type.IsA(typeof(T));

    /// <summary>
    /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified style and culture-specific
    /// format.
    /// </summary>
    /// <param name="convertible">The <see cref="IConvertible"/> instance to convert.</param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> interface implementation that supplies culture-specific formatting information.
    /// </param>
    /// <param name="style">
    /// A bitwise combination of enumeration values that indicates the style elements that can be present.
    /// </param>
    /// <returns>A 32-bit signed integer equivalent to the number specified in <c>convertible</c>.</returns>
    public static int ToInt32(this IConvertible convertible, IFormatProvider? formatProvider, NumberStyles style)
    {
        Require.NotNull(convertible, nameof(convertible));

        string stringEquivalent = convertible.ToString(formatProvider);

        return int.Parse(stringEquivalent, style, formatProvider);
    }
}