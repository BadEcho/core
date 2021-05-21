//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Fenestra;
using BadEcho.Fenestra.ViewModels;

namespace BadEcho.Omnified.Vision.Statistics.ViewModels
{
    /// <summary>
    /// Defines a view model that facilitates the display of an individual statistic exported from an Omnified game.
    /// </summary>
    internal interface IStatisticViewModel : IViewModel, IModelProvider<Statistic>
    { }
}
