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

using BadEcho.Game.States;

namespace BadEcho.Game.Tests;

internal sealed class TestGameState : GameState
{
    private bool _reportedEntered;

    public TestGameState(Microsoft.Xna.Framework.Game game) 
        : base(game)
    { }

    public event EventHandler? Entered;

    protected override void UpdateCore(GameUpdateTime time, bool isActive)
    {
        if (TransitionStatus == TransitionStatus.Entered && !_reportedEntered)
        {
            Entered?.Invoke(this, EventArgs.Empty);
            _reportedEntered = true;
        }
    }

    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    { }
}
