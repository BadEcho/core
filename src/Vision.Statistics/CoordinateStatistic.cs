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
    /// Provides an individual statistic exported from an Omnified game concerning a coordinate
    /// triplet value.
    /// </summary>
    public sealed class CoordinateStatistic : Statistic
    {
        /// <summary>
        /// Gets or sets the value for the x-coordinate.
        /// </summary>
        /// <seealso cref="WholeStatistic.Value" href="Remarks">On the nature of statistic numeric property values.</seealso>
        public double X
        { get; set; }

        /// <summary>
        /// Gets or sets the value for the y-coordinate.
        /// </summary>
        /// <seealso cref="WholeStatistic.Value">On the nature of statistic numeric property values.</seealso>
        public double Y
        { get; set; }

        /// <summary>
        /// Gets or sets the value for the z-coordinate.
        /// </summary>
        /// <seealso cref="WholeStatistic.Value">On the nature of statistic numeric property values.</seealso>
        public double Z
        { get; set; }
    }
}
