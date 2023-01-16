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

public class CircleTests
{
    private readonly Circle _circle;

    public CircleTests()
    {
        _circle = new Circle(200, 200, 200);
    }

    [Fact]
    public void Contains_InsidePoint_ReturnsTrue()
    {
        var inside = new PointF(180, 180);

        Assert.True(_circle.Contains(inside));
    }

    [Fact]
    public void Contains_OutsidePoint_ReturnsFalse()
    {
        var outside = new PointF(200, 500);

        Assert.False(_circle.Contains(outside));
    }

    [Fact]
    public void Contains_InsideCircle_ReturnsTrue()
    {
        var inside = new Circle(200, 200, 100);

        Assert.True(_circle.Contains(inside));
    }

    [Fact]
    public void Contains_OutsideCircle_ReturnsFalse()
    {
        var outside = new Circle(-20, -20, 50);

        Assert.False(_circle.Contains(outside));
    }

    [Fact]
    public void Intersects_OverlappingCircle_ReturnsTrue()
    {
        var overlapping = new Circle(50, 50, 100);

        Assert.True(_circle.Intersects(overlapping));
    }

    [Fact]
    public void Intersects_NonoverlappingCircle_ReturnsFalse()
    {
        var nonoverlapping = new Circle(-20, -20, 50);

        Assert.False(_circle.Intersects(nonoverlapping));
    }

    [Fact]
    public void ShapeIntersects_OverlappingRectangle_ReturnsTrue()
    {
        var overlapping = new RectangleF(100, 100, 150, 150);

        Assert.True(ShapeIntersects(overlapping));
    }

    [Fact]
    public void ShapeIntersects_NonoverlappingRectangle_ReturnsFalse()
    {
        var nonoverlapping = new RectangleF(-50, -50, 20, 20);

        Assert.False(ShapeIntersects(nonoverlapping));
    }

    private bool ShapeIntersects(IShape other)
    {
        IShape shape = _circle;

        return shape.Intersects(other);
    }
}
