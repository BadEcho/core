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
        private bool _isCritical;

        /// <summary>
        /// Gets or sets a value indicating if updates to the bound statistic is a critical event.
        /// </summary>
        public bool IsCritical
        {
            get => _isCritical;
            set => NotifyIfChanged(ref _isCritical, value);
        }

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
        protected override void OnUnbound(WholeStatistic model)
        {
            base.OnUnbound(model);

            Value = default;
        }
    }
}