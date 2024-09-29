//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Game.AI;

/// <summary>
/// Provides a model for a transition between states of finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of states.</typeparam>
internal sealed class TransitionModel<T>
{
    /// <summary>
    /// Gets a descriptor that identifies the target state of the transition.
    /// </summary>
    public T? Target
    { get; init; }

    /// <summary>
    /// Gets a collection of conditions that must be satisfied for the transition to occur.
    /// </summary>
    public ICollection<Func<T, bool>> Conditions
    { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating if the transition will occur when registered components can no longer
    /// continue processing.
    /// </summary>
    public bool OnComponentsDone 
    { get; set; }

    /// <summary>
    /// Gets the amount of time a state must be running before a transition can proceed.
    /// </summary>
    public TimeSpan Duration 
    { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is TransitionModel<T> otherModel && otherModel.Target.Equals<T>(Target);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Target);
}