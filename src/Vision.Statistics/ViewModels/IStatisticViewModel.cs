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

using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;

namespace BadEcho.Vision.Statistics.ViewModels;

/// <summary>
/// Defines a view model that facilitates the display of an individual statistic exported from an Omnified game.
/// </summary>
internal interface IStatisticViewModel : IViewModel, IModelProvider<IStatistic>
{
    /// <summary>
    /// Gets or sets the displayed name for the bound statistic.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets any formatting that should be applied to the value of the bound statistic.
    /// </summary>
    string Format { get; set; }
}