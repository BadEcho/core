//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a view model that facilitates the display of an individual fractional statistic exported from an Omnified game.
    /// </summary>
    internal sealed class FractionalStatisticViewModel : StatisticViewModel<FractionalStatistic>
    {
        private int _currentValue;
        private int _maximumValue;

        /// <summary>
        /// Gets or sets the displayed current numeric value for the bound statistic.
        /// </summary>
        public int CurrentValue
        {
            get => _currentValue;
            set => NotifyIfChanged(ref _currentValue, value);
        }

        /// <summary>
        /// Gets or sets the displayed maximum numeric value for the bound statistic.
        /// </summary>
        public int MaximumValue
        {
            get => _maximumValue;
            set => NotifyIfChanged(ref _maximumValue, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(FractionalStatistic model)
        {
            base.OnBinding(model);

            CurrentValue = model.CurrentValue;
            MaximumValue = model.MaximumValue;
        }

        /// <inheritdoc/>
        protected override void OnUnbound(FractionalStatistic model)
        {
            base.OnUnbound(model);

            CurrentValue = default;
            MaximumValue = default;
        }
    }
}