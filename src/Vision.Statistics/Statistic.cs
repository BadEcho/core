//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Omnified.Vision.Statistics;

/// <summary>
/// Provides an individual statistic exported from an Omnified game.
/// </summary>
public abstract class Statistic : IStatistic
{
    /// <inheritdoc/>
    public string Name
    { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string Format
    { get; init; } = "{0:N0}";

    /// <inheritdoc/>
    /// <remarks>
    /// We override the equality methods to establish the name of the statistic to essentially be its identity,
    /// and to allow for in-place rebindings of the view models responsible for displaying statistics with updated
    /// statistical data.
    /// </remarks>
    public override bool Equals(object? obj) 
        => obj is Statistic other && Name == other.Name;

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Name);
}