//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Vision.Statistics;

/// <summary>
/// Provides an individual statistic exported from an Omnified game concerning a fractional,
/// numeric value.
/// </summary>
/// <remarks>
/// The term "fractional" has no bearing on the actual data type of the statistic. Rather, a statistic is
/// "fractional" if it is expressed using values that have some sort of relationship to each other, such as
/// a character's health, which consists of a current and maximum value.
/// </remarks>
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

    /// <summary>
    /// Gets or sets the primary (first half of a gradient) color that represents the statistic visually.
    /// </summary>
    public string PrimaryBarColor
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary (second half of a gradient) color that represents the statistic visually.
    /// </summary>
    public string SecondaryBarColor
    { get; set; } = string.Empty;
}