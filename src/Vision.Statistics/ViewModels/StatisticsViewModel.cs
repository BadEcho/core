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

using BadEcho.Presentation.ViewModels;

namespace BadEcho.Vision.Statistics.ViewModels;

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