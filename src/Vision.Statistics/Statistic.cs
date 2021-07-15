//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensions;

namespace BadEcho.Omnified.Vision.Statistics
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
        { get; init; } = string.Empty;

        /// <inheritdoc/>
        /// <remarks>
        /// We override the equality methods to establish the name of the statistic to essentially be its identity,
        /// and to allow for in-place rebindings of the view models responsible for displaying statistics with updated
        /// statistical data.
        /// </remarks>
        public override bool Equals(object? obj) 
            => obj is Statistic other && Name == other.Name;

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.GetHashCode(Name);
    }
}
