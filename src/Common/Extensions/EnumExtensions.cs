//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;
using BadEcho.Properties;

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to simplify the use of enumerations.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts this integer value into its corresponding member of the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The type of <see cref="Enum"/> to convert to.</typeparam>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>A <typeparamref name="TEnum"/> representation of <c>value</c>.</returns>
    public static TEnum ToEnum<TEnum>(this int value) 
        where TEnum : Enum
    {
        EnsureIsInteger<TEnum>();

        return Unsafe.As<int, TEnum>(ref value);
    }

    /// <summary>
    /// Converts this enumeration member into its corresponding integer value.
    /// </summary>
    /// <typeparam name="TEnum">The type of <see cref="Enum"/> to convert from.</typeparam>
    /// <param name="member">The enumeration member to convert.</param>
    /// <returns>The <see cref="int"/> value of <c>member</c>.</returns>
    public static int ToInt32<TEnum>(this TEnum member)
        where TEnum : Enum
    {
        EnsureIsInteger<TEnum>();

        return Unsafe.As<TEnum, int>(ref member);
    }

    private static void EnsureIsInteger<TEnum>()
        where TEnum : Enum
    {
        if (Unsafe.SizeOf<TEnum>() != Unsafe.SizeOf<int>())
            throw new InvalidOperationException(Strings.EnumIntegralTypeNotInteger);
    }
}