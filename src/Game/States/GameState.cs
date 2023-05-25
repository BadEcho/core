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

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a scene for a game with independently managed content.
/// </summary>
public abstract class GameState
{
    /// <summary>
    /// Gets a value indicating if this state's scene is active and can receive input.
    /// </summary>
    public bool IsActive 
    { get; internal set; }


    /// <summary>
    /// Gets a value indicating this state has the topmost scene in the z-order.
    /// </summary>
    public bool IsTopmost 
    { get; internal set; }
     
    /// <summary>
    /// Gets the manager orchestrating this and other game states.
    /// </summary>
    protected StateManager Manager
    { get; }

    /// <summary>
    /// Loads resources needed by the state and prepares it to be drawn to the screen.
    /// </summary>
    public void Load()
    {

    }

    /// <summary>
    /// Unloads resources previously loaded by the state, called when the state no longer needs to be drawn
    /// to the screen.
    /// </summary>
    public void Unload()
    {

    }

    /// <summary>
    /// Performs any necessary updates to the state, including its position, transitional state, and other state-specific
    /// concerns.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public virtual void Update(GameUpdateTime time)
    {
       
    }

    /// <summary>
    /// Draws the scene associated with the state to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the state.</param>
    public abstract void Draw(SpriteBatch spriteBatch);
}
