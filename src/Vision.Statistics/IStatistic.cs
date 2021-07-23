//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Statistics
{
    /// <summary>
    /// Defines an individual statistic exported from an Omnified game.
    /// </summary>
    public interface IStatistic
    {
        /// <summary>
        /// Gets or sets the display name of the statistic.
        /// </summary>
        string Name { get; }
    }
}
