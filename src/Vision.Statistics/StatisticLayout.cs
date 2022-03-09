//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Presentation;

namespace BadEcho.Vision.Statistics;

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