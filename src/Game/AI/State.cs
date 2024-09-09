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
using BadEcho.Logging;

namespace BadEcho.Game.AI;

/// <summary>
/// Provides a possible state of a finite-state machine.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of the state.</typeparam>
public sealed class State<T>
    where T : notnull
{
    private readonly List<T> _transitionTargets = [];
    private readonly Dictionary<T, TimeSpan> _transitionDurations = [];
    private readonly Dictionary<T, List<Func<bool>>> _transitionConditions = []; 

    private readonly List<Action<T>> _enterActions;
    private readonly List<Action<T>> _exitActions;
    private readonly List<Action<GameUpdateTime>> _updateActions;
    private readonly List<Component> _updateComponents;

    private TimeSpan _timeRunning;

    internal State(StateModel<T> model)
    {
        Require.NotNull(model, nameof(model));

        if (model.Identifier == null)
            throw new InvalidOperationException(Strings.StateHasNoIdentifier);

        Identifier = model.Identifier;
        Next = model.Identifier;

        _enterActions = [..model.EnterActions];
        _exitActions = [..model.ExitActions];
        _updateActions = [..model.UpdateActions];
        _updateComponents = model.ComponentFactories.Select(f => f()).ToList();

        foreach (Func<Component> factory in model.ComponentFactories)
        {
            _updateComponents.Add(factory());
        }

        foreach (TransitionModel<T> transitionModel in model.Transitions)
        {
            if (transitionModel.Target == null)
                throw new InvalidOperationException(Strings.StateTransitionHasNoTargetIdentifier);

            _transitionTargets.Add(transitionModel.Target);

            if (transitionModel.Duration != default)
                _transitionDurations.Add(transitionModel.Target, transitionModel.Duration);

            _transitionConditions.Add(transitionModel.Target, [..transitionModel.Conditions]);
        }
    }

    /// <summary>
    /// Gets a descriptor that identifies the state.
    /// </summary>
    public T Identifier
    { get; }

    /// <summary>
    /// Gets a descriptor for the state that the finite-state machine should transition to next.
    /// </summary>
    public T Next
    { get; private set; }

    /// <summary>
    /// Activates the state, executing all configured enter actions.
    /// </summary>
    public void Enter()
    {
        _timeRunning = TimeSpan.Zero;

        Logger.Debug(Strings.StateEntering.InvariantFormat(Identifier));

        foreach (Action<T> enterAction in _enterActions)
        {
            enterAction(Identifier);
        }
    }

    /// <summary>
    /// Deactivates the state, executing all configured exit actions.
    /// </summary>
    public void Exit()
    {
        Logger.Debug(Strings.StateExiting.InvariantFormat(Identifier));

        foreach (Action<T> exitAction in _exitActions)
        {
            exitAction(Identifier);
        }
    }

    /// <summary>
    /// Called while the state is active to execute all configured update actions and to check if a state transition
    /// is to occur. 
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    /// <returns>True if a transition to the next state should occur; otherwise, false.</returns>
    public bool Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));
        
        _timeRunning += time.ElapsedGameTime;

        foreach (Action<GameUpdateTime> updateAction in _updateActions)
        {
            updateAction(time);
        }

        foreach (T transitionTarget in _transitionTargets)
        {
            if (_transitionDurations.TryGetValue(transitionTarget, out TimeSpan duration))
            {
                if (_timeRunning < duration)
                    continue;
            }

            if (_transitionConditions.TryGetValue(transitionTarget, out List<Func<bool>>? conditions))
            {
                if (conditions.Any(condition => !condition()))
                    continue;
            }

            Next = transitionTarget;
            return true;
        }

        return false;
    }
}
