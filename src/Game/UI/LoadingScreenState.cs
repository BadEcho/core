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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a self-contained user interface configuration game state that acts a loading screen, used to distract the player from 
/// the fact that the game takes so damn long to load.
/// </summary>
public abstract class LoadingScreenState : ScreenState
{
    private readonly IEnumerable<GameState> _statesToLoad;
    private bool _otherStatesUnloaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingScreenState"/> class.
    /// </summary>
    /// <param name="statesToLoad">The game states loaded by this interface.</param>
    /// <param name="device">The graphics device that will power the rendering surface.</param>
    protected LoadingScreenState(IEnumerable<GameState> statesToLoad, GraphicsDevice device)
        : base(device)
    {
        Require.NotNull(statesToLoad, nameof(statesToLoad));

        _statesToLoad = statesToLoad;
        ActivationTime = TimeSpan.FromSeconds(0.5);
    }

    /// <inheritdoc/>
    public override void Update(GameUpdateTime time)
    {
        base.Update(time);

        if (_otherStatesUnloaded)
        {
            foreach (GameState stateToLoad in _statesToLoad)
            {
                Manager?.AddState(stateToLoad);
            }

            Manager?.Game.ResetElapsedTime();
            Manager?.RemoveState(this);
        }

        if (ActivationStatus == ActivationStatus.Activated && Manager?.States.Count == 1)
            _otherStatesUnloaded = true;
    }

    /// <inheritdoc/>
    protected override void LoadContent(ContentManager contentManager)
    {
        base.LoadContent(contentManager);

        if (Manager == null)
            return;
                
        foreach (GameState state in Manager.States)
        {
            state.Exit();
        }       
    }
}
