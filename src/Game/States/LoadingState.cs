using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that acts a loading screen, used to distract the player from the fact that the game takes so
/// damn long to load.
/// </summary>
public sealed class LoadingState : GameState
{
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        
    }

    protected override void LoadContent(ContentManager contentManager)
    {
        
    }
}
