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

namespace BadEcho.Game.AI;

/// <summary>
/// Defines a fluent interface for configuring a possible state of a finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of the state.</typeparam>
public interface IStateBuilder<T> : IStateMachineBuilder<T>
{
    /// <summary>
    /// Registers a callback that runs when the state becomes active.
    /// </summary>
    /// <param name="enterAction">
    /// The method executed by the state when activated, providing it with the state identifier.
    /// </param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> OnEnter(Action<T> enterAction);

    /// <summary>
    /// Registers a callback that runs when the state is no longer active.
    /// </summary>
    /// <param name="exitAction">
    /// The method executed by the state when deactivated, providing it with the state identifier.
    /// </param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> OnExit(Action<T> exitAction);
    
    /// <summary>
    /// Registers a callback that runs if this is the active state when <see cref="StateMachine{T}.Update"/> is called.
    /// </summary>
    /// <param name="action">The method executed by this state at the time of update.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> Executes(Action<GameUpdateTime> action);

    /// <summary>
    /// Adds a transition to another state.
    /// </summary>
    /// <param name="state">The state to transition to.</param>
    /// <returns>
    /// A <see cref="IStateTransitionBuilder{T}"/> instance that can be used to further configure the transition.
    /// </returns>
    IStateTransitionBuilder<T> TransitionTo(T state);
}
