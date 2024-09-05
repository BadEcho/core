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
/// Defines a fluent interface for configuring a finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of the finite-state machine's states.</typeparam>
public interface IStateMachineBuilder<T>
    where T : notnull
{
    /// <summary>
    /// Registers a new state.
    /// </summary>
    /// <param name="state">A descriptor for the state.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to configure the new state.</returns>
    IStateBuilder<T> Add(T state);

    /// <summary>
    /// Generates a new finite-state machine from this configuration.
    /// </summary>
    /// <param name="initialState">Descriptor for the initial state of the finite-state machine.</param>
    /// <returns>A <see cref="StateMachine{T}"/> instance based on this configuration.</returns>
    StateMachine<T> Build(T initialState);
}
