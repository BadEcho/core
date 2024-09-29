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

using BadEcho.Game.Properties;
using BadEcho.Extensions;

namespace BadEcho.Game.AI;

/// <summary>
/// Provides a finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of states.</typeparam>
public sealed class StateMachine<T> : Component
    where T : notnull
{
    private readonly Dictionary<T, State<T>> _states = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachine{T}"/> class.
    /// </summary>
    /// <param name="states">The possible states of the finite-state machine.</param>
    /// <param name="initialState">The starting state of the finite-state machine.</param>
    public StateMachine(IEnumerable<State<T>> states, T initialState)
    {
        Require.NotNull(states, nameof(states));

        foreach (State<T> state in states)
        {
            _states.Add(state.Identifier, state);
        }

        if (!_states.TryGetValue(initialState, out State<T>? currentState))
            throw new ArgumentException(Strings.StateMachineMissingState.InvariantFormat(initialState), nameof(initialState));

        CurrentState = currentState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Gets the currently active state.
    /// </summary>
    public State<T> CurrentState
    { get; private set; }

    /// <inheritdoc/>
    public override bool Update(IEntity entity, GameUpdateTime time)
    {
        bool shouldTransition = CurrentState.Update(entity, time);

        if (shouldTransition)
            TransitionToNext();

        return true;
    }

    /// <summary>
    /// Executes logic related to the current state and any pending transitions.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void Update(GameUpdateTime time)
    {
        bool shouldTransition = CurrentState.Update(time);

        if (shouldTransition)
            TransitionToNext();
    }
    
    private void TransitionToNext()
    {
        CurrentState.Exit();
        CurrentState = _states[CurrentState.Next];
        CurrentState.Enter();
    }
}
