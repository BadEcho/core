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

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state for hosting core gameplay.
/// </summary>
public abstract class GameplayState : GameState
{
    public bool IsPaused
    { get; protected set; }

    public sealed override void Update(GameUpdateTime time, bool isTopmost)
    {
        base.Update(time, true);

        
    }
}
