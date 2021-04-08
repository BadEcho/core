//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Fenestra.Converters
{
    /// <summary>
    /// Provides a value converter that produces <see cref="Style"/> objects to and from provided binary equivalents.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Style))]
    public sealed class BooleanToStyleConverter : ValueConverter<bool,Style?>
    {
        /// <summary>
        /// Gets or sets the <see cref="Style"/> that <c>true</c> input values are converted into.
        /// </summary>
        public Style? StyleWhenTrue
        { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that <c>false</c> input values are converted into.
        /// </summary>
        public Style? StyleWhenFalse
        {  get; set; }

        /// <inheritdoc/>
        protected override Style? Convert(bool value, object parameter, CultureInfo culture)
            => value ? StyleWhenTrue : StyleWhenFalse;

        /// <inheritdoc/>
        protected override bool ConvertBack(Style? value, object parameter, CultureInfo culture)
            => StyleWhenTrue == value;
    }
}