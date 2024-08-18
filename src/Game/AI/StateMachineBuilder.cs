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
using BadEcho.Game.Properties;

namespace BadEcho.Game.AI;

public sealed class StateMachineBuilder<T> : IStateOrTransitionBuilder<T>
    where T : notnull
{
    private readonly Dictionary<T,StateModel<T>> _states = [];

    private TransitionModel<T> _currentTransition = new();
    private StateModel<T> _currentState = new();

    IStateBuilder<T> IStateBuilder<T>.OnEnter(Action<T> enterAction)
    {
        Require.NotNull(enterAction, nameof(enterAction));

        _currentState.EnterActions.Add(enterAction);

        return this;
    }

    IStateBuilder<T> IStateBuilder<T>.OnExit(Action<T> exitAction)
    {
        Require.NotNull(exitAction, nameof(exitAction));

        _currentState.ExitActions.Add(exitAction);

        return this;
    }

    IStateBuilder<T> IStateBuilder<T>.Executes(Action<GameUpdateTime> action)
    {
        Require.NotNull(action, nameof(action));

        _currentState.UpdateActions.Add(action);

        return this;
    }
    
    IStateTransitionBuilder<T> IStateBuilder<T>.TransitionTo(T state)
    {
        Require.NotNull(state, nameof(state));

        var newTransition = new TransitionModel<T> { Target = state };
        
        if (_currentState.Transitions.Contains(newTransition))
        {
            throw new ArgumentException(
                Strings.StateAlreadyHasTransition.InvariantFormat(_currentState.Identifier, state),
                nameof(state));
        }

        _currentState.Transitions.Add(newTransition);
        _currentTransition = newTransition;

        return this;
    }

    IStateOrTransitionBuilder<T> IStateTransitionBuilder<T>.IfTrue(Func<bool> condition)
    {
        Require.NotNull(condition, nameof(condition));

        _currentTransition.Conditions.Add(condition);

        return this;
    }

    IStateOrTransitionBuilder<T> IStateTransitionBuilder<T>.After(TimeSpan duration)
    {
        _currentTransition.Duration = duration;

        return this;
    }

    /// <inheritdoc/>
    public IStateBuilder<T> Add(T state)
    {
        Require.NotNull(state, nameof(state));

        var newState = new StateModel<T> { Identifier = state };

        if (!_states.TryAdd(state, newState))
            throw new ArgumentException(Strings.StateMachineAlreadyHasState.InvariantFormat(state));

        _currentState = newState;
        
        return this;
    }

    /// <inheritdoc/>
    public StateMachine<T> Build()
    {

    }
}
