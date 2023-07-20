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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that hosts a graphical user interface for the purpose of either displaying information, receiving
/// input from the user, or both.
/// </summary>
public sealed class UserInterfaceState : GameState
{


    /// <inheritdoc />
    public override void Draw(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override void LoadContent(ContentManager contentManager)
    {
        throw new NotImplementedException();
    }
}
