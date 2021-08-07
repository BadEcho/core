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
