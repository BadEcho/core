﻿//-----------------------------------------------------------------------
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
    /// Provides an individual statistic exported from an Omnified game concerning a fractional,
    /// numeric value.
    /// </summary> 
    public sealed class FractionalStatistic : Statistic
    {
        /// <summary>
        /// Gets or sets the current numeric value for the statistic.
        /// </summary>
        public int CurrentValue
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum numeric value the statistic can be.
        /// </summary>
        public int MaximumValue
        { get; set; }
    }
}
