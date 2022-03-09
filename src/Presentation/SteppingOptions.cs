//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Presentation;

/// <summary>
/// Provides stepping sequence options related to the timing of a binding.
/// </summary>
internal sealed class SteppingOptions : TransientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SteppingOptions"/> class. 
    /// </summary>
    /// <param name="binding">The binding that will have its changes propagated in a stepped fashion.</param>
    public SteppingOptions(IBinding binding)
        : base(binding)
    { }

    /// <summary>
    /// Gets or sets the total duration of a binding update stepping sequence.
    /// </summary>
    public TimeSpan SteppingDuration
    { get; set; }
        
    /// <summary>
    /// Gets or sets the amount of change incurred by a single step.
    /// </summary>
    public double StepAmount
    { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of steps required in order for a stepping sequence to be executed.
    /// </summary>
    public int MinimumSteps
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the target property is an integer, as opposed to a floating-point number.
    /// </summary>
    public bool IsInteger
    { get; set; }
}