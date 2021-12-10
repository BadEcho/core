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

namespace BadEcho.Omnified.Vision.Statistics;

/// <summary>
/// Provides an individual statistic exported from an Omnified game concerning a coordinate
/// triplet value.
/// </summary>
/// <remarks>
/// <para>
/// The default floating-point numeric type used in .NET is <c>double</c>, however the majority of
/// coordinate data we export from Omnified processes are actually of the <c>float</c> variety.
///</para>
/// <para>
/// While converting from a float to a double directly may result in a value that is different from what
/// we expect, the value this property is being set to will actually be getting sourced from a message file,
/// for which all numbers that are contained within are actually string representations of the original values.
/// Therefore, there will be no chance of anything like a widening conversion happening.
/// </para>
/// </remarks>
public sealed class CoordinateStatistic : Statistic
{
    /// <summary>
    /// Gets or sets the value for the x-coordinate.
    /// </summary>
    public double X
    { get; set; }

    /// <summary>
    /// Gets or sets the value for the y-coordinate.
    /// </summary>
    public double Y
    { get; set; }

    /// <summary>
    /// Gets or sets the value for the z-coordinate.
    /// </summary>
    public double Z
    { get; set; }
}