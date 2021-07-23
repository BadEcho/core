//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Fenestra;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.Views
{
    /// <summary>
    /// Provides inheritable layout properties for statistic views.
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
        /// Gets a value indicating whether statistics should be displayed in compact form.
        /// </summary>
        /// <param name="source">The <see cref="UIElement"/> to get the value from.</param>
        /// <returns>Value indicating if statistics should be displayed in compact form.</returns>
        public static bool GetIsCompact(UIElement source)
        {
            Require.NotNull(source, nameof(source));

            return (bool)source.GetValue(IsCompactProperty);
        }

        /// <summary>
        /// Sets a value indicating whether statistics should be displayed in compact form.
        /// </summary>
        /// <param name="source">The <see cref="UIElement"/> to set the value on.</param>
        /// <param name="value">The value indicating whether statistics should be displayed in compact form.</param>
        public static void SetIsCompact(UIElement source, bool value)
        {
            Require.NotNull(source, nameof(source));

            source.SetValue(IsCompactProperty, value);
        }
    }
}
