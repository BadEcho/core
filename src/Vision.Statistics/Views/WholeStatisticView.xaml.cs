//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace BadEcho.Omnified.Vision.Statistics.Views
{
    /// <summary>
    /// Provides a view for an individual whole statistic exported from an Omnified game.
    /// </summary>
    public partial class WholeStatisticView
    {
        /// <summary>
        /// Identifies the attached property which indicates whether the statistic should be displayed in compact form.
        /// </summary>
        public static readonly DependencyProperty IsCompactProperty
            = StatisticLayout.IsCompactProperty.AddOwner(typeof(WholeStatisticView),
                                                         new FrameworkPropertyMetadata(
                                                             false,
                                                             FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// Initializes a new instance of the <see cref="WholeStatisticView"/> class.
        /// </summary>
        public WholeStatisticView() 
            => InitializeComponent();
    }
}
