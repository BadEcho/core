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