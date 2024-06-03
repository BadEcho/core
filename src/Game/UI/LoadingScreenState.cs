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

using BadEcho.Game.States;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a self-contained user interface game state that acts a loading screen, used to distract the player from 
/// the fact that the game takes so damn long to load.
/// </summary>
public abstract class LoadingScreenState : ScreenState
{
    private bool _otherStatesUnloaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingScreenState"/> class.
    /// </summary>
    /// <param name="game">The game this state is for.</param>
    protected LoadingScreenState(Microsoft.Xna.Framework.Game game)
        : base(game)
    {
        TransitionTime = TimeSpan.FromSeconds(0.5);
    }

    /// <inheritdoc/>
    protected override void UpdateCore(GameUpdateTime time, bool isActive)
    {
        base.UpdateCore(time, isActive);

        if (_otherStatesUnloaded)
        {
            foreach (GameState stateToLoad in GetStatesToLoad())
            {
                Manager?.AddState(stateToLoad);
            }

            Manager?.Game.ResetElapsedTime();
            Manager?.RemoveState(this);
        }

        if (TransitionStatus == TransitionStatus.Entered && Manager?.States.Count == 1)
            _otherStatesUnloaded = true;
    }

    /// <inheritdoc/>
    protected override void OnLoad(StateManager manager)
    {
        Require.NotNull(manager, nameof(manager));

        foreach (GameState state in manager.States)
        {
            state.Close();
        }       
        
        base.OnLoad(manager);
    }

    /// <summary>
    /// Retrieves the game states to load with this user interface.
    /// </summary>
    /// <returns>The sequence of <see cref="GameState"/> instances to load.</returns>
    protected abstract IEnumerable<GameState> GetStatesToLoad();
}
