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

using BadEcho.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state for hosting core gameplay.
/// </summary>
public abstract class GameplayState : GameState
{
    public bool IsPaused
    { get; protected set; }

    protected float PauseAlpha
    { get; set; } = 0.1f;

    public sealed override void Update(GameUpdateTime time, bool isActive)
    {
        base.Update(time, true);

        IsPaused = !isActive;

        if (!IsPaused)
            UpdateGameplay(time);
    }

    protected sealed override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));        

        DrawGameplay(spriteBatch);        
    }

    /// <summary>
    /// Performs any necessary updates to gameplay hosted by this state.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    protected abstract void UpdateGameplay(GameUpdateTime time);

    protected abstract void DrawGameplay(ConfiguredSpriteBatch spriteBatch);
}
