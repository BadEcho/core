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

    internal State(StateModel<T> model)
    {
        Require.NotNull(model, nameof(model));

        if (model.Identifier == null)
            throw new InvalidOperationException(Strings.StateHasNoIdentifier);

        Identifier = model.Identifier;

        _enterActions = [..model.EnterActions];
        _exitActions = [..model.ExitActions];
        _updateActions = [..model.UpdateActions];

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

    public void Update(GameUpdateTime time)
    {

    }
}
