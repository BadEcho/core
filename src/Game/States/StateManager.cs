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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.States;

public sealed class StateManager : GameComponent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateManager"/> class.
    /// </summary>
    /// <param name="game">The game that the component should be attached to.</param>
    public StateManager(Microsoft.Xna.Framework.Game game)
        : base(game)
    { }

    /// <summary>
    /// Removes the specified game state from the collection of loaded states.
    /// </summary>
    /// <param name="state">The game state to unload from this manager.</param>
    public void RemoveState(GameState state)
    { }
}
