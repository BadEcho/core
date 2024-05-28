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

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state for hosting core gameplay.
/// </summary>
public abstract class GameplayState : GameState
{
    private readonly Brush _pauseOverlay = new(Color.Black);

    public bool IsPaused
    { get; protected set; }

    /// <summary>
    /// Gets or sets the transparency of the overlay that appears when the game is paused.
    /// </summary>
    public float PauseOverlayAlpha
    { get; set; } = 0.5f;

    /// <inheritdoc/>
    protected override void UpdateCore(GameUpdateTime time, bool isActive) 
        => IsPaused = !isActive;

    /// <inheritdoc/>
    protected sealed override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));        

        DrawGameplay(spriteBatch);

        if (IsPaused)
        {
            _pauseOverlay.Color = Color.Black * PauseOverlayAlpha;
            _pauseOverlay.Draw(spriteBatch, spriteBatch.GraphicsDevice.Viewport.Bounds);
        }
    }

    protected abstract void DrawGameplay(ConfiguredSpriteBatch spriteBatch);
}
