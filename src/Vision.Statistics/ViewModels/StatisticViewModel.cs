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
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Provides a base view model that facilitates the display of an individual statistic exported from an Omnified game.
    /// </summary>
    internal abstract class StatisticViewModel<TStatistic> : ViewModel<IStatistic,TStatistic>, IStatisticViewModel
        where TStatistic : IStatistic
    {
        private string? _name;

        /// <inheritdoc/>
        public string Name
        {
            get => _name ?? string.Empty;
            set => NotifyIfChanged(ref _name, value);
        }
        
        /// <inheritdoc/>
        protected override void OnBinding(TStatistic model)
        {
            Require.NotNull(model, nameof(model));

            Name = model.Name;
        }

        /// <inheritdoc/>
        protected override void OnUnbound(TStatistic model) 
            => Name = string.Empty;
    }
}