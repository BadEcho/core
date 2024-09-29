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

/// <summary>
/// Provides a builder for configuring finite-state machines.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of states.</typeparam>
public sealed class StateMachineBuilder<T> : IStateOrTransitionBuilder<T>
    where T : notnull
{
    private readonly HashSet<StateModel<T>> _states = [];

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

    IStateBuilder<T> IStateBuilder<T>.Executes(Func<Component> factory)
    {
        Require.NotNull(factory, nameof(factory));

        _currentState.ComponentFactories.Add(factory);

        return this;
    }

    IStateBuilder<T> IStateBuilder<T>.Executes<TComponent>()
    {
        _currentState.ComponentFactories.Add(() => new TComponent());

        return this;
    }

    IStateTransitionBuilder<T> IStateBuilder<T>.TransitionsTo(T state)
    {
        Require.NotNull(state, nameof(state));

        var newTransition = new TransitionModel<T> { Target = state };
        
        if (!_currentState.Transitions.Add(newTransition))
        {
            throw new ArgumentException(
                Strings.StateAlreadyHasTransition.InvariantFormat(_currentState.Identifier, state),
                nameof(state));
        }

        _currentTransition = newTransition;

        return this;
    }

    IStateOrTransitionBuilder<T> IStateTransitionBuilder<T>.WhenTrue(Func<T, bool> condition)
    {
        Require.NotNull(condition, nameof(condition));

        _currentTransition.Conditions.Add(condition);

        return this;
    }

    IStateOrTransitionBuilder<T> IStateTransitionBuilder<T>.WhenComponentsDone()
    {
        _currentTransition.OnComponentsDone = true;

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

        if (!_states.Add(newState))
            throw new ArgumentException(Strings.StateMachineAlreadyHasState.InvariantFormat(state));

        _currentState = newState;
        
        return this;
    }

    /// <inheritdoc/>
    public StateMachine<T> Build(T initialState)
    {
        var states = _states.Select(stateModel => new State<T>(stateModel));

        return new StateMachine<T>(states, initialState);
    }
}
