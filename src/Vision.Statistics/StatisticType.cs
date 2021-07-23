//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Specifies the type of statistic exported from an Omnified game.
    /// </summary>
    public enum StatisticType
    {
        /// <summary>
        /// A whole, numeric value statistic.
        /// </summary>
        Whole,
        /// <summary>
        /// A fractional, numeric value statistic.
        /// </summary>
        Fractional,
        /// <summary>
        /// A coordinate triplet value statistic.
        /// </summary>
        Coordinate,
        /// <summary>
        /// A grouping of similar statistics.
        /// </summary>
        Group
    }
}
