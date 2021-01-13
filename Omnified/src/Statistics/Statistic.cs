//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Statistics
{
    /// <summary>
    /// Provides an individual statistic exported from an Omnified game.
    /// </summary>
    public abstract class Statistic
    {
        /// <summary>
        /// Gets or sets the display name of the statistic.
        /// </summary>
        public string Name
        { get; set; } = string.Empty;
    }
}
