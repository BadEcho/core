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

using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels;

/// <summary>
/// Provides a base view model that facilitates the display of an individual statistic exported from an Omnified game.
/// </summary>
/// <typeparam name="TStatistic">The type of statistic bound to the view model for display on a view.</typeparam>
internal abstract class StatisticViewModel<TStatistic> : ViewModel<IStatistic,TStatistic>, IStatisticViewModel
    where TStatistic : IStatistic
{
    private string _name = string.Empty;
    private string _format = string.Empty;

    /// <inheritdoc/>
    public string Name
    {
        get => _name;
        set => NotifyIfChanged(ref _name, value);
    }

    /// <inheritdoc/>
    public string Format
    {
        get => _format;
        set => NotifyIfChanged(ref _format, value);
    }
        
    /// <inheritdoc/>
    protected override void OnBinding(TStatistic model)
    {
        Require.NotNull(model, nameof(model));
            
        Name = model.Name;
        Format = model.Format;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(TStatistic model)
    {
        Name = string.Empty;
        Format = string.Empty;
    }
}