//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Vision.Statistics;

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