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

using System.Collections.ObjectModel;
using BadEcho.Odin.Extensions;

namespace BadEcho.Omnified.Vision.Statistics;

/// <summary>
/// Provides a grouping of similar statistics exported from an Omnified game.
/// </summary>
public sealed class StatisticGroup : IStatistic
{
    /// <inheritdoc/>
    public string Name
    { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string Format
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets a sequence of individual statistics that compose this group.
    /// </summary>
    public IEnumerable<IStatistic> Statistics 
    { get; init; } = new Collection<IStatistic>();

    /// <inheritdoc/>
    /// <remarks>
    /// We override the equality methods to establish the name of the statistic to essentially be its identity,
    /// and to allow for in-place rebindings of the view models responsible for displaying statistics with updated
    /// statistical data.
    /// </remarks>
    public override bool Equals(object? obj)
        => obj is StatisticGroup other && Name == other.Name;

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Name);
}