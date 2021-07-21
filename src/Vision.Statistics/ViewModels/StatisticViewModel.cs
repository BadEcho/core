//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a base view model that facilitates the display of an individual statistic exported from an Omnified game.
    /// </summary>
    internal abstract class StatisticViewModel<TStatistic> : ViewModel<Statistic,TStatistic>, IStatisticViewModel
        where TStatistic : Statistic
    {
        private string? _name;
        private bool _isCompact;

        /// <summary>
        /// Gets or sets the displayed name for the bound statistic.
        /// </summary>
        public string Name
        {
            get => _name ?? string.Empty;
            set => NotifyIfChanged(ref _name, value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the statistic should be displayed in compact form, with the name and value(s) of the statistic
        /// packed together on top of each other, as opposed to the normal colon-separated horizontal layout.
        /// </summary>
        /// <remarks>This property is not influenced by the statistic bound to the view model, it must be set externally.</remarks>
        public bool IsCompact
        {
            get => _isCompact;
            set => NotifyIfChanged(ref _isCompact, value);
        }

        /// <inheritdoc/>
        protected override void OnBinding(TStatistic model)
        {
            Require.NotNull(model, nameof(model));

            Name = $"{model.Name}:";
        }

        /// <inheritdoc/>
        protected override void OnUnbound(TStatistic model)
        {
            Name = string.Empty;
            IsCompact = false;
        }
    }
}