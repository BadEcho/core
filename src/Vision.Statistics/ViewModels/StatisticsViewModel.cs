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

using BadEcho.Fenestra.ViewModels;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels;

/// <summary>
/// Provides a view model that displays statistics exported from an Omnified game.
/// </summary>
internal sealed class StatisticsViewModel : PolymorphicCollectionViewModel<IStatistic,IStatisticViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsViewModel"/> class.
    /// </summary>
    public StatisticsViewModel()
        : base(new CollectionViewModelOptions {AsyncBatchBindings = false})
    {
        RegisterDerivation<WholeStatistic, WholeStatisticViewModel>();
        RegisterDerivation<FractionalStatistic, FractionalStatisticViewModel>();
        RegisterDerivation<CoordinateStatistic, CoordinateStatisticViewModel>();
        RegisterDerivation<StatisticGroup, StatisticGroupViewModel>();
    }
}