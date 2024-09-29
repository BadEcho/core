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
using Microsoft.Xna.Framework.Content;
using Xunit;

namespace BadEcho.Game.Tests;

public class SpriteSheetTests : IClassFixture<ContentManagerFixture>
{
    private readonly ContentManager _content;

    public SpriteSheetTests(ContentManagerFixture contentFixture)
        => _content = contentFixture.Content;

    [Fact]
    public void Load_StickMan_ReturnsValid()
    {
        SpriteSheet sheet = _content.Load<SpriteSheet>("Images\\StickMan");

        Assert.NotNull(sheet);
        Assert.Equal(4, sheet.ColumnCount);
        Assert.Equal(1, sheet.RowCount);

        var idle = sheet.GetAnimation("Idle");

        Assert.NotNull(idle);
    }

    [Fact]
    public void Animate_StickMan_ValidFrameAreas()
    {
        SpriteSheet sheet = _content.Load<SpriteSheet>("Images\\StickMan");
        
        var idleAnimation = sheet.GetAnimation("Idle");
        var game = new TestGame();

        Assert.Equal(new RectangleF(0, 0, 16, 32), sheet.GetFrameRectangle(idleAnimation));

        idleAnimation.Play();

        idleAnimation.Update(new GameUpdateTime(game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(201))));
        Assert.Equal(new RectangleF(16, 0, 16, 32), sheet.GetFrameRectangle(idleAnimation));
        idleAnimation.Update(new GameUpdateTime(game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(201))));
        Assert.Equal(new RectangleF(32, 0, 16, 32), sheet.GetFrameRectangle(idleAnimation));
        idleAnimation.Update(new GameUpdateTime(game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(201))));
        Assert.Equal(new RectangleF(48, 0, 16, 32), sheet.GetFrameRectangle(idleAnimation));
    }
}
