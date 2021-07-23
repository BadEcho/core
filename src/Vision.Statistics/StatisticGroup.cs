//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using BadEcho.Odin.Extensions;

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Provides a grouping of similar statistics exported from an Omnified game.
    /// </summary>
    public sealed class StatisticGroup : IStatistic
    {
        /// <inheritdoc/>
        public string Name
        { get; init; } = string.Empty;

        /// <summary>
        /// Gets a sequence of individual statistics that comprise this group.
        /// </summary>
        public IEnumerable<IStatistic> Statistics 
        { get; init; } = new Collection<IStatistic>();

        /// <inheritdoc/>
        /// <remarks>
        /// We override the equality methods to establish the name of the statistic to essentially be its identity,
        /// and to allow for in-place rebindings of the view models responsible for displaying statistics with updated
        /// statistical data.
        /// </remarks>
        public override bool Equals(object? obj)
            => obj is StatisticGroup other && Name == other.Name;

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.GetHashCode(Name);
    }
}
