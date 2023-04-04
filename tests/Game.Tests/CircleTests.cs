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

    //[Fact]
    //public void Minkowski()
    //{
    //    var c1 = new Circle(4, 7, 2.5f);
    //    var c2 = new Circle(8, 5, 3);

    //    var displacement = c1.Center.Displace(c2.Center);

    //    Vector2 d2;
    //    if (displacement != Vector2.Zero)
    //    {
    //        var d3 = displacement;
    //        d3.Normalize();
    //        d2 = d3 *(c1.Radius + c2.Radius);

    //    }
    //    else
    //    {
    //        d2 = -Vector2.UnitY * (c1.Radius + c2.Radius);
    //    }

    //    var pen = displacement - d2;

    //    var movedCircle = new Circle(c1.Center - pen, 2.5f);
    //}

    //[Fact]
    //public void C()
    //{
    //    var c2 = new Circle(2, 3.3f, 5);
    //    var c1 = new Circle(1, 2, 4);

    //    var d = c1.Center.Displace(c2.Center);

    //    var distance = d.Length();

    //    var combinedR = c1.Radius + c2.Radius;

    //    var depth = combinedR - distance;

    //    var direction = d;

    //    direction.Normalize();
    //    //var pen = direction * depth;

    //    var pen = c1.CalculatePenetration(c2);
        
    //    Vector2 dd;
    //    if (d != Vector2.Zero)
    //    {
            
    //        var dn = d;
    //        dn.Normalize();

    //        dd = dn * (c1.Radius + c2.Radius);

    //    }
    //    else
    //    {
    //        dd = -Vector2.UnitY * (c1.Radius + c2.Radius);
    //    }

    //    var p = dd - d;
    //}

    //[Fact]
    //public void MinRectCircle()
    //{

    //    var rect
    //        //= new RectangleF(2, 2, 5, 5);
    //        //= new RectangleF(2, 6, 3f, 3f);
    //        // 50,450,100,100 
    //        // 300, 100, 150, 150 center x
    //        // 100, 150, 150, 150 centery
    //        = new RectangleF(150, 150, 100, 100); // test case for UnitY


    //    //t.CalculatePenetration(e);

    //    var circ = _circle;//new Circle(3.5f, 11.5f, 4f);
    //    Vector2 p;

    //    var closest = rect.GetPointClosestTo(circ.Center);
    //    var centerToClosest = circ.Center.Displace(closest);

    //    if (rect.Contains(circ.Center) || centerToClosest == Vector2.Zero)
    //    {
    //        var d = circ.Center.Displace(rect.Center);

    //        Vector2 dd; 
    //        if (d != Vector2.Zero)
    //        {
    //            var dx = new Vector2(d.X, 0);
    //            var dy = new Vector2(0, d.Y);
    //            var dl = d.Length();
    //            var dxm =  dx.Length();
    //            var dym = dy.Length();

    //            dx.Normalize();
    //            dy.Normalize();
    //            var predx = (circ.Radius + rect.Width / 2);
    //            dx *=  (predx - dxm);
    //            dy *= (circ.Radius + rect.Height / 2) - dym;

    //            if (dxm < dym)
    //            {
    //                dd = dx; // DD is PEN VEC +
    //                d.Y = 0;
    //            }
    //            else
    //            {
    //                dd = dy;
    //                d.X = 0;
    //            }
    //        }
    //        else
    //        {
    //            dd = Vector2.UnitY * (circ.Radius + rect.Height / 2);
    //        }

    //        p = d - dd;
    //    }
    //    else
    //    {
    //        var b = centerToClosest;
    //        var centerClosestLength = centerToClosest.Length();
    //        // Get direction of penetration
    //                        b.Normalize();
    //                        //centerToClosest.Normalize();
    //        // Vector is d
    //        //p = circ.Radius * b - centerToClosest;
    //        p = centerToClosest.ToUnit() * (circ.Radius - centerClosestLength);
    //    }

    //    var newrect = new RectangleF(rect.Location - (-1 * p), rect.Size);
    //    var newcirc = new Circle(circ.Center - p, circ.Radius);
    //}

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
