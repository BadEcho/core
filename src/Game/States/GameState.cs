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
    public void Load()
    {

    }

    public void Unload()
    {

    }

    public void Update(GameUpdateTime time)
    {

    }

    public void Draw(SpriteBatch spriteBatch)
    {

    }
}
