//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
    private readonly Circle _circle = new(200, 200, 200);

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
    public void Contains_TouchingCircle_ReturnsFalse()
    {
        var touching = new Circle(-200, 200, 200);

        Assert.False(_circle.Contains(touching));
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

    [Theory]
    [InlineData(200, 200, 100)] // Sharing center point
    [InlineData(100, 100, 100)] // Not sharing center point
    public void CalculatePenetration_OverlappingCircle_ReturnsSeparatingVector(float otherX, float otherY, float otherRadius)
    {
        var overlapping = new Circle(otherX, otherY, otherRadius);

        Assert.True(_circle.Intersects(overlapping));

        var penetration = _circle.CalculatePenetration(overlapping);
        var adjustedCircle = new Circle(_circle.Center + penetration, _circle.Radius);

        Assert.False(adjustedCircle.Intersects(overlapping));
    }

    [Theory]
    [InlineData(50, 300, 100)]  // Circle center not within rectangle
    [InlineData(100, 150, 150)] // Circle center within rectangle, displace in direction of x-axis
    [InlineData(150, 100, 150)] // Circle center within rectangle, displace in direction of y-axis
    [InlineData(150, 150, 100)] // Sharing center point
    public void CalculatePenetration_OverlappingRectangle_ReturnsSeparatingVector(float otherX, float otherY, float otherSize)
    {
        var overlapping = new RectangleF(otherX, otherY, otherSize, otherSize);
        
        Assert.True(ShapeIntersects(overlapping));

        var penetration = _circle.CalculatePenetration(overlapping);
        var adjustedCircle = new Circle(_circle.Center + penetration, _circle.Radius);

        Assert.False(ShapeIntersects(adjustedCircle, overlapping));
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

    private static bool ShapeIntersects(IShape source, IShape other)
    {
        return source.Intersects(other);
    }

    private bool ShapeIntersects(IShape other)
        => ShapeIntersects(_circle, other);
}
