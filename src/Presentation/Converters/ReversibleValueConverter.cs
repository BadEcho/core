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

using System.Globalization;
using System.Windows.Data;

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a base <see cref="IValueConverter"/> implementation which offers the ability to reverse the direction of
/// source-to-target conversions performed by the converter.
/// </summary>
/// <remarks>
/// Reversing a value converter which converts from type A to type B will yield a converter which converts from type B to type A.
/// </remarks>
/// <typeparam name="TInput">The type of value produced by the associated source binding.</typeparam>
/// <typeparam name="TOutput">The type of value produced by the value converter.</typeparam>
public abstract class ReversibleValueConverter<TInput,TOutput> : ValueConverter<TInput,TOutput>
{
    /// <summary>
    /// Gets or sets a value indicating if the direction of the conversion performed by this converter should
    /// be reversed.
    /// </summary>
    public bool Reversed
    { get; set; }

    /// <inheritdoc/>
    public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !Reversed
            ? base.Convert(value, targetType, parameter, culture)
            : ConvertBack(value, targetType, parameter, culture);
    }
}