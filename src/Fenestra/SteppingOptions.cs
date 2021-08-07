//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides stepping sequence options related to timing.
    /// </summary>
    internal sealed class SteppingOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SteppingOptions"/> class. 
        /// </summary>
        /// <param name="steppingDuration">The total duration of a binding update stepping sequence.</param>
        /// <param name="minimumSteps">
        /// The minimum number of steps required in order for a stepping sequence to be executed.
        /// </param>
        public SteppingOptions(TimeSpan steppingDuration, int minimumSteps)
        {
            SteppingDuration = steppingDuration;
            MinimumSteps = minimumSteps;
        }

        /// <summary>
        /// Gets the total duration of a binding update stepping sequence.
        /// </summary>
        public TimeSpan SteppingDuration
        { get; }

        /// <summary>
        /// Gets the minimum number of steps required in order for a stepping sequence to be executed.
        /// </summary>
        public int MinimumSteps
        { get; }
    }
}
