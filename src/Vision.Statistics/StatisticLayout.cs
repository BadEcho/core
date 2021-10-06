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

using System.Windows;
using BadEcho.Fenestra;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides inheritable layout properties for statistic displays.
    /// </summary>
    public static class StatisticLayout
    {
        /// <summary>
        /// Identifies the attached property which indicates whether statistics should be displayed in compact form.
        /// </summary>
        public static readonly DependencyProperty IsCompactProperty
            = DependencyProperty.RegisterAttached(NameOf.ReadDependencyPropertyName(() => IsCompactProperty),
                                                  typeof(bool),
                                                  typeof(StatisticLayout),
                                                  new FrameworkPropertyMetadata(false,
                                                                                FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// Gets the value of the <see cref="IsCompactProperty"/> attached property for a given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="source">The element from which the property value is read.</param>
        /// <returns>Value indicating if statistics should be displayed in compact form.</returns>
        public static bool GetIsCompact(UIElement source)
        {
            Require.NotNull(source, nameof(source));

            return (bool)source.GetValue(IsCompactProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="IsCompactProperty"/> attached property on a given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="source">The element to which the attached property is written.</param>
        /// <param name="value">The value indicating whether statistics should be displayed in compact form.</param>
        public static void SetIsCompact(UIElement source, bool value)
        {
            Require.NotNull(source, nameof(source));

            source.SetValue(IsCompactProperty, value);
        }
    }
}
