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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Tests;

/// <suppressions>
/// ReSharper disable ObjectCreationAsStatement
/// </suppressions>
internal sealed class TestGame : Microsoft.Xna.Framework.Game
{
    private bool _isExiting;

    public TestGame()
    {
        new GraphicsDeviceManager(this);
    }

    public event EventHandler? Initialized;
    public new event EventHandler? Exiting;

    public Func<bool> ExitCondition
    { get; set; } = () => true;

    protected override void Initialize()
    {
        base.Initialize();

        Initialized?.Invoke(this, EventArgs.Empty);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_isExiting)
            return;

        if (ExitCondition())
        {
            _isExiting = true;
            Exiting?.Invoke(this, EventArgs.Empty);
            Exit();

            return;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (_isExiting)
            return;

        base.Draw(gameTime);
    }
}
