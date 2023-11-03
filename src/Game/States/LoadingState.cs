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

using Microsoft.Xna.Framework.Content;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that acts a loading screen, used to distract the player from the fact that the game takes so
/// damn long to load.
/// </summary>
public sealed class LoadingState : GameState
{
    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {

    }

    /// <inheritdoc/>
    protected override void LoadContent(ContentManager contentManager)
    {
        
    }
}
