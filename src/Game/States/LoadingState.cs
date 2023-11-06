//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Game.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that acts a loading screen, used to distract the player from the fact that the game takes so
/// damn long to load.
/// </summary>
public sealed class LoadingState : GameState
{
    private readonly IEnumerable<GameState> _statesToLoad;
    private readonly UserInterface _loadingInterface;
    private readonly Screen _screen;

    private bool _otherStatesUnloaded;


    public LoadingState(IEnumerable<GameState> statesToLoad, UserInterface loadingInterface, GraphicsDevice device)
    {
        Require.NotNull(statesToLoad, nameof(statesToLoad));
        Require.NotNull(loadingInterface, nameof(loadingInterface));
        Require.NotNull(device, nameof(device));

        _statesToLoad = statesToLoad;
        _loadingInterface = loadingInterface;

        _screen = new Screen(device);

        ActivationTime = TimeSpan.FromSeconds(0.5);
    }

    /// <inheritdoc/>
    public override void Update(GameUpdateTime time)
    {
        _screen.Update();

        ContentOrigin = _screen.Content.LayoutBounds.Location;

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
        if (Manager == null)
            return;

        foreach (GameState state in Manager.States)
        {
            state.Exit();
        }

        _loadingInterface.Attach(_screen, contentManager);
    }

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
        => _screen.Draw(spriteBatch);
}
