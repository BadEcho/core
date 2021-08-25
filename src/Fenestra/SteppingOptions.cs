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

using System;
using BadEcho.Odin;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides stepping sequence options related to the timing of a binding.
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
        /// <param name="binding">The binding that will have its changes propagated in a stepped fashion.</param>
        public SteppingOptions(TimeSpan steppingDuration, int minimumSteps, IBinding binding)
        {
            Require.NotNull(binding, nameof(binding));

            SteppingDuration = steppingDuration;
            MinimumSteps = minimumSteps;
            Binding = binding;
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

        /// <summary>
        /// Gets the binding that will have its changes propagated in a stepped fashion.
        /// </summary>
        public IBinding Binding
        { get; }
    }
}
