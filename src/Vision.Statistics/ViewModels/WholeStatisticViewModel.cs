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

using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a view model that facilitates the display of an individual whole statistic exported from an Omnified game.
    /// </summary>
    internal sealed class WholeStatisticViewModel : StatisticViewModel<WholeStatistic>
    {
        private double _value;
        private bool _isCritical;

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeStatisticViewModel"/> class.
        /// </summary>
        /// <param name="statistic">The whole statistic to bind to the view model.</param>
        public WholeStatisticViewModel(WholeStatistic statistic)
        {
            Require.NotNull(statistic, nameof(statistic));

            Bind(statistic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeStatisticViewModel"/> class.
        /// </summary>
        public WholeStatisticViewModel()
        { }

        /// <summary>
        /// Gets or sets a value indicating if updates to the bound statistic are critical events.
        /// </summary>
        public bool IsCritical
        {
            get => _isCritical;
            set => NotifyIfChanged(ref _isCritical, value);
        }

        /// <summary>
        /// Gets or sets the displayed numeric value for the bound whole statistic.
        /// </summary>
        public double Value
        {
            get => _value;
            set => NotifyIfChanged(ref _value, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(WholeStatistic model)
        {
            base.OnBinding(model);

            IsCritical = model.IsCritical;
            Value = model.Value;
        }

        /// <inheritdoc/>
        protected override void OnUnbound(WholeStatistic model)
        {
            base.OnUnbound(model);

            IsCritical = false;
            Value = default;
        }
    }
}