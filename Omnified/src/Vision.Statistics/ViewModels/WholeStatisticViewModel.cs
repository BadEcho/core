//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a view model that facilitates the display of an individual whole statistic exported from an Omnified game.
    /// </summary>
    internal sealed class WholeStatisticViewModel : StatisticViewModel<WholeStatistic>
    {
        private int _value;

        /// <summary>
        /// Gets or sets the displayed whole numeric value for the bound statistic.
        /// </summary>
        public int Value
        {
            get => _value;
            set => NotifyIfChanged(ref _value, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(WholeStatistic model)
        {
            base.OnBinding(model);

            Value = model.Value;
        }

        /// <inheritdoc/>
        protected override void OnUnbind(WholeStatistic model)
        {
            base.OnUnbind(model);

            Value = default;
        }
    }
}