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

using System.Windows;

namespace BadEcho.Vision.Statistics.Views;

/// <summary>
/// Provides a view for an individual coordinate statistic exported from an Omnified game.
/// </summary>
public partial class CoordinateStatisticView
{
    /// <summary>
    /// Identifies the attached property which indicates whether the statistic should be displayed in compact form.
    /// </summary>
    public static readonly DependencyProperty IsCompactProperty
        = StatisticLayout.IsCompactProperty.AddOwner(typeof(CoordinateStatisticView),
                                                     new FrameworkPropertyMetadata(
                                                         false,
                                                         FrameworkPropertyMetadataOptions.Inherits));
    /// <summary>
    /// Initializes a new instance of the <see cref="CoordinateStatisticView"/> class.
    /// </summary>
    public CoordinateStatisticView() 
        => InitializeComponent();
}