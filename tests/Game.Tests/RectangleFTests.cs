//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Xunit;

namespace BadEcho.Game.Tests;

public class RectangleFTests
{
    private readonly RectangleF _rect;

    public RectangleFTests()
    {   
        _rect = new RectangleF(0, 0, 300, 600);
    }

    [Fact]
    public void Intersects_OverlappingRectangle_ReturnsTrue()
    {
        var overlapping = new RectangleF(100, 100, 200, 400);

        Assert.True(_rect.Intersects(overlapping));
    }

    [Fact]
    public void Intersects_NonoverlappingRectangle_ReturnsFalse()
    {
        var nonoverlapping = new RectangleF(500, 800, 20, 30);

        Assert.False(_rect.Intersects(nonoverlapping));
    }

    [Fact]
    public void ShapeIntersects_OverlappingRectangle_ReturnsTrue()
    {
        var overlapping = new RectangleF(100, 100, 200, 400);

        IShape shape = _rect;

        Assert.True(shape.Intersects(overlapping));
    }

    [Fact]
    public void ShapeIntersects_NonoverlappingRectangle_ReturnsFalse()
    {
        var nonoverlapping = new RectangleF(500, 800, 20, 30);

        IShape shape = _rect;

        Assert.False(shape.Intersects(nonoverlapping));
    }
    
    [Fact]
    public void ShapeIntersects_OverlappingCircle_ReturnsTrue()
    {
        var overlapping = new Circle(new PointF(400, 700), 200);

        IShape shape = _rect;

        Assert.True(shape.Intersects(overlapping));
    }

    [Fact]
    public void ShapeIntersects_NonoverlappingCircle_ReturnsFalse()
    {
        var nonoverlapping = new Circle(new PointF(500, 800), 20);

        IShape shape = _rect;

        Assert.False(shape.Intersects(nonoverlapping));
    }
}