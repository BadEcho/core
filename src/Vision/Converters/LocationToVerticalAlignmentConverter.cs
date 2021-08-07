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

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BadEcho.Fenestra.Converters;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.Converters
{
    /// <summary>
    /// Provides a value converter that produces <see cref="VerticalAlignment"/> values equivalent to the
    /// <see cref="AnchorPointLocation"/> values provided, and vice versa.
    /// </summary>
    [ValueConversion(typeof(AnchorPointLocation), typeof(VerticalAlignment))]
    public sealed class LocationToVerticalAlignmentConverter : ValueConverter<AnchorPointLocation, VerticalAlignment>
    {
        /// <summary>
        /// Gets or sets a value indicating if values intended for the binding source that are returned during a <see cref="BindingMode.TwoWay"/>
        /// conversion will be towards the left of the screen (an assumption we must make as no horizontal orientation information is
        /// present in a <see cref="VerticalAlignment"/> value), as opposed to the right of the screen.
        /// </summary>
        public bool DefaultsToLeft
        { get; set; } = true;

        /// <inheritdoc/>
        protected override VerticalAlignment Convert(AnchorPointLocation value, object parameter, CultureInfo culture) 
            => value switch
            {
                AnchorPointLocation.BottomCenter => VerticalAlignment.Bottom,
                AnchorPointLocation.BottomLeft => VerticalAlignment.Bottom,
                AnchorPointLocation.BottomRight => VerticalAlignment.Bottom,
                AnchorPointLocation.TopLeft => VerticalAlignment.Top,
                AnchorPointLocation.TopCenter => VerticalAlignment.Top,
                AnchorPointLocation.TopRight => VerticalAlignment.Top,
                _ => VerticalAlignment.Top
            };

        /// <inheritdoc/>
        protected override AnchorPointLocation ConvertBack(VerticalAlignment value, object parameter, CultureInfo culture)
            => value switch
            {
                VerticalAlignment.Stretch => DefaultsToLeft ? AnchorPointLocation.TopLeft : AnchorPointLocation.TopRight,
                VerticalAlignment.Top => DefaultsToLeft ? AnchorPointLocation.TopLeft : AnchorPointLocation.TopRight,
                VerticalAlignment.Center => DefaultsToLeft ? AnchorPointLocation.TopLeft : AnchorPointLocation.TopRight,
                VerticalAlignment.Bottom => DefaultsToLeft ? AnchorPointLocation.BottomLeft : AnchorPointLocation.BottomRight,
                _ => DefaultsToLeft ? AnchorPointLocation.TopLeft : AnchorPointLocation.TopRight
            };
    }
}
