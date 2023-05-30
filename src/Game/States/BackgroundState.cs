using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that acts as a backdrop behind all other states. Unlike other states,
/// it will not begin to deactivate if it's not at the top of the z-order, as it is meant to be
/// </summary>
public sealed class BackgroundState : GameState
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }

    protected override void LoadContent(ContentManager contentManager)
    {
        throw new NotImplementedException();
    }
}
