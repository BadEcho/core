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
using BadEcho.Game.UI;

namespace BadEcho.Game.Tests;

internal sealed class TestScreenState : ScreenState
{
    public TestScreenState(Microsoft.Xna.Framework.Game game) 
        : base(game)
    { }

    protected override IPanel LoadControls(StateManager manager)
        => new StackPanel();
}
