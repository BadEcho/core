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
using System.Windows;
using System.Windows.Data;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Provides a value converter that converts a provided <see cref="double"/> value by returning the result of an arithmetic
/// operation involving both the input value as well as a bindable operand value.
/// </summary>
[ValueConversion(typeof(double), typeof(double))]
public sealed class ArithmeticConverter : FreezableValueConverter<double,double>
{
    /// <summary>
    /// Identifies the <see cref="Operand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OperandProperty
        = DependencyProperty.Register(nameof(Operand),
                                      typeof(double),
                                      typeof(ArithmeticConverter));
    /// <summary>
    /// Gets or sets the operand value used in the arithmetic operation performed on an input value.
    /// </summary>
    public double Operand
    {
        get => (double) GetValue(OperandProperty);
        set => SetValue(OperandProperty, value);
    }

    /// <summary>
    /// Gets or sets a <see cref="ArithmeticOperation"/> value that specifies the type of arithmetic operation performed during
    /// input value conversion.
    /// </summary>
    public ArithmeticOperation Operation
    { get; set; }

    /// <inheritdoc/>
    protected override double Convert(double value, object? parameter, CultureInfo culture)
    {
        if (Operand.ApproximatelyEquals(0.0) && Operation == ArithmeticOperation.Division)
            return value;

        return Operation switch
        {
            ArithmeticOperation.Addition => value + Operand,
            ArithmeticOperation.Subtraction => value - Operand,
            ArithmeticOperation.Multiplication => value * Operand,
            ArithmeticOperation.Division => value / Operand,
            _ => throw new InvalidOperationException(Strings.InvalidArithmeticConverterOperation)
        };
    }

    /// <inheritdoc/>
    protected override double ConvertBack(double value, object? parameter, CultureInfo culture)
    {
        if (Operand.ApproximatelyEquals(0.0) && Operation == ArithmeticOperation.Multiplication)
            return value;

        return Operation switch
        {
            ArithmeticOperation.Addition => value - Operand,
            ArithmeticOperation.Subtraction => value + Operand,
            ArithmeticOperation.Multiplication => value / Operand,
            ArithmeticOperation.Division => value * Operand,
            _ => throw new InvalidOperationException(Strings.InvalidArithmeticConverterOperation)
        };
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new ArithmeticConverter();
}
