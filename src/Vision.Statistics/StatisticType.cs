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

namespace BadEcho.Vision.Statistics;

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