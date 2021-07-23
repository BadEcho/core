//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;
using BadEcho.Omnified.Vision.Statistics.Properties;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a view model that displays statistics exported from an Omnified game.
    /// </summary>
    internal sealed class StatisticsViewModel : CollectionViewModel<IStatistic,IStatisticViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewModel"/> class.
        /// </summary>
        public StatisticsViewModel()
            : base(new CollectionViewModelOptions {AsyncBatchBindings = false})
        { }

        /// <inheritdoc/>
        public override IStatisticViewModel CreateChild(IStatistic model)
        {
            Require.NotNull(model, nameof(model));

            return model switch
            {
                WholeStatistic whole => new WholeStatisticViewModel(whole),
                FractionalStatistic fractional => new FractionalStatisticViewModel(fractional),
                CoordinateStatistic coordinate => new CoordinateStatisticViewModel(coordinate),
                StatisticGroup group => new StatisticGroupViewModel(group),
                _ => throw new ArgumentException(Strings.StatisticTypeUnsupportedViewModel, nameof(model))
            };
        }

        /// <inheritdoc/>
        public override void UpdateChild(IStatistic model)
        {
            Require.NotNull(model, nameof(model));

            switch (model)
            {
                case WholeStatistic whole:
                    UpdateChild<WholeStatisticViewModel, WholeStatistic>(whole);
                    break;

                case FractionalStatistic fractional:
                    UpdateChild<FractionalStatisticViewModel, FractionalStatistic>(fractional);
                    break;

                case CoordinateStatistic coordinate:
                    UpdateChild<CoordinateStatisticViewModel, CoordinateStatistic>(coordinate);
                    break;

                case StatisticGroup group:
                    UpdateChild<StatisticGroupViewModel, StatisticGroup>(group);
                    break;
            }
        }

        /// <inheritdoc/>
        public override void OnChangeCompleted()
        { }

        private void UpdateChild<TStatisticViewModel, TStatistic>(TStatistic model)
            where TStatistic : IStatistic
            where TStatisticViewModel : ViewModel<TStatistic>, IStatisticViewModel
        {
            var existingChild = FindChild<TStatisticViewModel>(model);

            if (existingChild == null)
            {
                throw new ArgumentException(Strings.CannotUpdateUnboundStatistic.InvariantFormat(model.Name),
                                            nameof(model));
            }

            existingChild.Bind(model);
        }
    }
}
