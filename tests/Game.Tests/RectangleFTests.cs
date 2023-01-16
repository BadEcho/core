//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
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

        Assert.True(ShapeIntersects(overlapping));
    }

    [Fact]
    public void ShapeIntersects_NonoverlappingRectangle_ReturnsFalse()
    {
        var nonoverlapping = new RectangleF(500, 800, 20, 30);

        Assert.False(ShapeIntersects(nonoverlapping));
    }
    
    [Fact]
    public void ShapeIntersects_OverlappingCircle_ReturnsTrue()
    {
        var overlapping = new Circle(new PointF(400, 700), 200);

        Assert.True(ShapeIntersects(overlapping));
    }

    [Fact]
    public void ShapeIntersects_NonoverlappingCircle_ReturnsFalse()
    {
        var nonoverlapping = new Circle(new PointF(500, 800), 20);

        Assert.False(ShapeIntersects(nonoverlapping));
    }

    [Fact]
    public void ShapeContains_InnerCircle_ReturnsTrue()
    {
        IShape inner = new Circle(150, 300, 100);

        Assert.True(_rect.Contains(inner));
    }

    [Fact]
    public void ShapeContains_OuterCircle_ReturnsFalse()
    {
        IShape outer = new Circle(150, 300, 200);

        Assert.False(_rect.Contains(outer));
    }

    [Fact]
    public void ShapeContains_InnerRectangle_ReturnsTrue()
    {
        IShape inner = new RectangleF(50, 50, 200, 500);

        Assert.True(_rect.Contains(inner));
    }

    [Fact]
    public void ShapeContains_OuterRectangle_ReturnsFalse()
    {
        IShape outer = new RectangleF(50, 50, 400, 700);

        Assert.False(_rect.Contains(outer));
    }

    private bool ShapeIntersects(IShape other)
    {
        IShape shape = _rect;

        return shape.Intersects(other);
    }
}   