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

using BadEcho.Game.Routines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace BadEcho.Game.Tests;

public class PatrolTests : IClassFixture<ContentManagerFixture>
{
    private readonly TestGame _game = new();

    private readonly GameUpdateTime _time;
    private readonly Sprite _entity;

    public PatrolTests(ContentManagerFixture contentFixture)
    {
        var texture = contentFixture.Content.Load<Texture2D>("Images\\Circle");

        _entity = new Sprite(texture);
        _time = new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void Update_RightTriangle_MovesBetweenTwoPoints()
    {   // From the origin, this path forms a right triangle. A distance of five is needed to reach first point.
        var firstPoint = new Vector2(3, 4);
        var secondPoint = new Vector2(3, 0);

        _entity.Components.Add(new Patrol([firstPoint, secondPoint], 1.0f));

        // One unit per tick means five ticks to hit (a,c).
        for (int i = 0; i < 5; i++)
        {
            Assert.NotEqual(firstPoint, _entity.Position);

            _entity.Update(_time);
        }

        Assert.Equal(firstPoint, _entity.Position);

        // Four ticks to hit (a,b).
        for (int i = 0; i < 4; i++)
        {
            Assert.NotEqual(secondPoint, _entity.Position);

            _entity.Update(_time);
        }

        Assert.Equal(secondPoint, _entity.Position);

        // Should return to (a,c).
        for (int i = 0; i < 4; i++)
        {
            Assert.NotEqual(firstPoint, _entity.Position);

            _entity.Update(_time);
        }

        Assert.Equal(firstPoint, _entity.Position);
    }

    [Fact]
    public void Update_CircularFourPoints_LoopsFullCircle()
    {
        var firstPoint = new Vector2(1, 0);
        var secondPoint = new Vector2(2, 0);
        var thirdPoint = new Vector2(2, 1);
        var fourthPoint = new Vector2(1, 1);

        _entity.Components.Add(new Patrol([firstPoint, secondPoint, thirdPoint, fourthPoint],
                                          1.0f,
                                          LoopType.Circular));
        
        // One tick's worth of distance between all points.
        Assert.NotEqual(firstPoint, _entity.Position);
        
        _entity.Update(_time);
        Assert.Equal(firstPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(secondPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(thirdPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(fourthPoint, _entity.Position);

        // Should loop back to beginning.
        _entity.Update(_time);
        Assert.Equal(firstPoint, _entity.Position);
    }

    [Fact]
    public void Update_PingPongFourPoints_LoopsBackAndForth()
    {
        var firstPoint = new Vector2(1, 0);
        var secondPoint = new Vector2(2, 0);
        var thirdPoint = new Vector2(2, 1);
        var fourthPoint = new Vector2(1, 1);

        _entity.Components.Add(new Patrol([firstPoint, secondPoint, thirdPoint, fourthPoint],
                                          1.0f,
                                          LoopType.PingPong));
        
        // One tick's worth of distance between all points.
        Assert.NotEqual(firstPoint, _entity.Position);
        
        _entity.Update(_time);
        Assert.Equal(firstPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(secondPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(thirdPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(fourthPoint, _entity.Position);

        // Should loop in reverse to beginning.
        _entity.Update(_time);
        Assert.Equal(thirdPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(secondPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(firstPoint, _entity.Position);

        // And then back forwards...
        _entity.Update(_time);
        Assert.Equal(secondPoint, _entity.Position);
    }

    [Fact]
    public void Update_NoneFourPoints_DoesNotLoop()
    {
        var firstPoint = new Vector2(1, 0);
        var secondPoint = new Vector2(2, 0);
        var thirdPoint = new Vector2(2, 1);
        var fourthPoint = new Vector2(1, 1);

        _entity.Components.Add(new Patrol([firstPoint, secondPoint, thirdPoint, fourthPoint],
                                          1.0f,
                                          LoopType.None));
        
        // One tick's worth of distance between all points.
        Assert.NotEqual(firstPoint, _entity.Position);
        
        _entity.Update(_time);
        Assert.Equal(firstPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(secondPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(thirdPoint, _entity.Position);
        _entity.Update(_time);
        Assert.Equal(fourthPoint, _entity.Position);

        // Should stay at the end.
        _entity.Update(_time);
        Assert.Equal(fourthPoint, _entity.Position);
    }
}
