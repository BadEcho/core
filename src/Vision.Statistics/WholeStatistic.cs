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
    /// Provides an individual statistic exported from an Omnified game concerning a whole, numeric value.
    /// </summary>
    /// <remarks>
    /// The term "whole" has no bearing on the actual value type of the statistic, which may either be a whole
    /// number or one with fractional parts. Rather, a statistic is "whole" if it stands on its own, and has
    /// no relationship or bearing with another value, such as a character's experience level, which consists
    /// of only a single value.
    /// </remarks>
    public sealed class WholeStatistic : Statistic
    {
        /// <summary>
        /// Gets or set a value indicating if updates to this statistic are critical events.
        /// </summary>
        public bool IsCritical
        { get; set; }

        /// <summary>
        /// Gets or sets the numeric value for the statistic.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default floating-point numeric type used in .NET is <c>double</c>, however the majority of data we export from
        /// Omnified processes, if said data contains a fractional part, are actually of the <c>float</c> variety.
        ///</para>
        /// <para>
        /// While converting from a float to a double directly may result in a value that is different from what we expect, the value
        /// this property is being set to will actually be getting sourced from a message file, for which all numbers that are contained
        /// within are actually string representations of the original values. Therefore, there will be no chance of anything like a
        /// widening conversion happening.
        /// </para>
        /// </remarks>
        public double Value
        { get; set; }
    }
}
