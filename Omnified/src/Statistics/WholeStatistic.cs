//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Statistics
{
    /// <summary>
    /// Provides an individual statistic exported from an Omnified game concerning a whole, numeric value.
    /// </summary>
    public sealed class WholeStatistic : Statistic
    {
        /// <summary>
        /// Gets or sets the whole numeric value for the statistic.
        /// </summary>
        public int Value
        { get; set; }
    }
}
