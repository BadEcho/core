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
    /// Provides a value converter that produces <see cref="HorizontalAlignment"/> values equivalent to the
    /// <see cref="AnchorPointLocation"/> values provided, and vice versa.
    /// </summary>
    [ValueConversion(typeof(AnchorPointLocation), typeof(HorizontalAlignment))]
    public sealed class LocationToHorizontalAlignmentConverter : ValueConverter<AnchorPointLocation,HorizontalAlignment>
    {
        /// <summary>
        /// Gets a value indicating if values intended for the binding source that are returned during a <see cref="BindingMode.TwoWay"/>
        /// conversion will be towards the top of the screen (an assumption we must make as no vertical orientation information is
        /// present in a <see cref="HorizontalAlignment"/> value), as opposed to the bottom of the screen.
        /// </summary>
        public bool DefaultsToTop
        { get; set; } = true;

        /// <inheritdoc/>
        protected override HorizontalAlignment Convert(AnchorPointLocation value, object parameter, CultureInfo culture) 
            => value switch
            {
                AnchorPointLocation.BottomCenter => HorizontalAlignment.Center,
                AnchorPointLocation.BottomLeft => HorizontalAlignment.Left,
                AnchorPointLocation.BottomRight => HorizontalAlignment.Right,
                AnchorPointLocation.TopCenter => HorizontalAlignment.Center,
                AnchorPointLocation.TopLeft => HorizontalAlignment.Left,
                AnchorPointLocation.TopRight => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Left
            };

        /// <inheritdoc/>
        protected override AnchorPointLocation ConvertBack(HorizontalAlignment value, object parameter, CultureInfo culture) 
            => value switch
            {
                HorizontalAlignment.Stretch => DefaultsToTop ? AnchorPointLocation.TopLeft : AnchorPointLocation.BottomLeft,
                HorizontalAlignment.Left => DefaultsToTop ? AnchorPointLocation.TopLeft : AnchorPointLocation.BottomLeft,
                HorizontalAlignment.Center => DefaultsToTop ? AnchorPointLocation.TopCenter : AnchorPointLocation.BottomCenter,
                HorizontalAlignment.Right => DefaultsToTop ? AnchorPointLocation.TopRight : AnchorPointLocation.BottomRight,
                _ => DefaultsToTop ? AnchorPointLocation.TopLeft : AnchorPointLocation.BottomLeft
            };
    }
}
