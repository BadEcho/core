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
    where T : notnull
{
    /// <summary>
    /// Registers a callback that runs when the state becomes active.
    /// </summary>
    /// <param name="enterAction">The method executed by the state when activated.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> OnEnter(Action<T> enterAction);

    /// <summary>
    /// Registers a callback that runs when the state is no longer active.
    /// </summary>
    /// <param name="exitAction">The method executed by the state when deactivated.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> OnExit(Action<T> exitAction);
    
    /// <summary>
    /// Registers a callback that runs if this is the active state when <see cref="StateMachine{T}.Update(GameUpdateTime)"/>
    /// is called.
    /// </summary>
    /// <param name="action">The method executed by this state at the time of update.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> Executes(Action<GameUpdateTime> action);

    /// <summary>
    /// Registers a component to execute if this is the active state when <see cref="StateMachine{T}.Update(IEntity,GameUpdateTime)"/>
    /// is called.
    /// </summary>
    /// <param name="factory">A delegate that is used to create an instance of the component.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> Executes(Func<Component> factory);

    /// <summary>
    /// Registers a component to execute if this is the active state when <see cref="StateMachine{T}.Update(IEntity,GameUpdateTime)"/>
    /// is called.
    /// </summary>
    /// <typeparam name="TComponent">The type of component to execute.</typeparam>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateBuilder<T> Executes<TComponent>() where TComponent : Component, new();

    /// <summary>
    /// Adds a transition to another state.
    /// </summary>
    /// <param name="state">The state to transition to.</param>
    /// <returns>
    /// A <see cref="IStateTransitionBuilder{T}"/> instance that can be used to further configure the transition.
    /// </returns>
    IStateTransitionBuilder<T> TransitionsTo(T state);
}
    