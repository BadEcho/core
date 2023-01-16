//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a base <see cref="IValueConverter"/> implementation capable of being part of the visual tree which performs
/// validation of the values provided by the associated source binding in addition to making them available in their
/// expected strongly typed forms.
/// </summary>
/// <typeparam name="TInput">The type of value produced by the associated source binding.</typeparam>
/// <typeparam name="TOutput">The type of value produced by the value converter.</typeparam>
public abstract class FreezableValueConverter<TInput,TOutput> : Freezable, IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TInput)
            return DependencyProperty.UnsetValue;

        var inputValue = (TInput)value;

        return Convert(inputValue, parameter, culture);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TOutput)
            return DependencyProperty.UnsetValue;

        TOutput outputValue = (TOutput) value;

        return ConvertBack(outputValue, parameter, culture);
    }

    /// <summary>
    /// Converts the provided input value into its <typeparamref name="TOutput"/> typed form.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.Convert"/>
    /// <returns><c>value</c> in its <typeparamref name="TOutput"/> typed form.</returns>
    protected abstract TOutput Convert(TInput value, object parameter, CultureInfo culture);

    /// <summary>
    /// Converts the provided output value back into its <typeparamref name="TInput"/> typed form.
    /// </summary>
    /// <inheritdoc cref="IValueConverter.ConvertBack"/>
    /// <returns><c>value</c> in its <typeparamref name="TInput"/> typed form.</returns>
    protected abstract TInput ConvertBack(TOutput value, object parameter, CultureInfo culture);
}