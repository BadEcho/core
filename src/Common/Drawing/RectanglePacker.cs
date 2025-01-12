//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;

namespace BadEcho.Drawing;

/// <summary>
/// Provides a packer of rectangles into a larger, optimized space.
/// </summary>
/// <remarks>
/// This is based on algorithms originally written by Javiero Arevalo, which can be found at:
/// https://www.iguanademos.com/Jare/Articles.php?view=RectPlace
/// </remarks>
public sealed class RectanglePacker
{
    private readonly List<Point> _anchors = [Point.Empty];
    private readonly List<Rectangle> _packedRectangles = [];
    private readonly Size _maximumPackingSize;

    private Size _currentPackingSize = new (1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="RectanglePacker"/> class.
    /// </summary>
    /// <param name="maximumPackingSize">The maximum width and height of the area rectangles are packed into.</param>
    public RectanglePacker(Size maximumPackingSize)
    {
        _maximumPackingSize = maximumPackingSize;
    }

    /// <summary>
    /// Attempts to pack a rectangle of the specified size into the available packing space.
    /// </summary>
    /// <param name="size">The size of the rectangle to pack.</param>
    /// <param name="position">
    /// When this method returns, a <see cref="Point"/> value representing the position of the packed rectangle,
    /// or a default value if the packing failed.
    /// </param>
    /// <returns>True if a rectangle with a size of <c>size</c> was packed; otherwise, false.</returns>
    public bool TryPack(Size size, out Point position)
    {
        position = new Point();
        int anchorIndex = SelectAnchor(size, _currentPackingSize);

        if (anchorIndex == -1)
            return false;

        position = _anchors[anchorIndex];

        FitPosition(ref position, size);

        bool blocksAnchor = position.X + size.Width > _anchors[anchorIndex].X
                            && position.Y + size.Height > _anchors[anchorIndex].Y;
        
        if (blocksAnchor) 
            _anchors.RemoveAt(anchorIndex);

        InsertAnchor(position with { X = position.X + size.Width });
        InsertAnchor(position with { Y = position.Y + size.Height });

        _packedRectangles.Add(new Rectangle(position, size));

        return true;
    }

    private int SelectAnchor(Size size, Size currentPackingSize)
    {
        int anchorIndex = FindFreeAnchor(size, currentPackingSize);

        if (anchorIndex != -1)
        {
            _currentPackingSize = currentPackingSize;

            return anchorIndex;
        }

        bool canEnlargeWidth = currentPackingSize.Width < _maximumPackingSize.Width;
        bool canEnlargeHeight = currentPackingSize.Height < _maximumPackingSize.Height;
        bool shouldEnlargeHeight = !canEnlargeWidth || currentPackingSize.Height < currentPackingSize.Width;

        if (canEnlargeHeight && shouldEnlargeHeight)
        {
            Size newPackingSize
                = currentPackingSize with { Height = Math.Min(currentPackingSize.Height * 2, _maximumPackingSize.Height) };

            return SelectAnchor(size, newPackingSize);
        }

        if (canEnlargeWidth)
        {
            Size newPackingSize
                = currentPackingSize with { Width = Math.Min(currentPackingSize.Width * 2, _maximumPackingSize.Width) };

            return SelectAnchor(size, newPackingSize);
        }

        return -1;
    }

    private int FindFreeAnchor(Size size, Size currentPackingSize)
    {
        var potentialAnchor = new Rectangle(Point.Empty, size);

        for (int i = 0; i < _anchors.Count; i++)
        {
            Point anchor = _anchors[i];

            potentialAnchor.X = anchor.X;
            potentialAnchor.Y = anchor.Y;

            if (IsFree(potentialAnchor, currentPackingSize))
                return i;
        }

        return -1;
    }

    private bool IsFree(Rectangle area, Size currentPackingSize)
    {
        bool leavesArea = area.X < 0 || area.Y < 0 || area.Right > currentPackingSize.Width
                          || area.Bottom > currentPackingSize.Height;

        return !leavesArea && _packedRectangles.All(packedRectangle => !packedRectangle.IntersectsWith(area));
    }

    private void FitPosition(ref Point position, Size size)
    {
        var positionArea = new Rectangle(position, size);

        int leftmost = position.X;

        while (IsFree(positionArea, _maximumPackingSize))
        {
            leftmost = positionArea.X;
            --positionArea.X;
        }

        positionArea.X = position.X;

        int topmost = position.Y;
        
        while (IsFree(positionArea, _maximumPackingSize))
        {
            topmost = positionArea.Y;
            --positionArea.Y;
        }

        if (position.X - leftmost > position.Y - topmost)
            position.X = leftmost;
        else
            position.Y = topmost;
    }

    private void InsertAnchor(Point anchor)
    {
        int insertIndex
            = _anchors.BinarySearch(anchor, Comparison.Using<Point>((l, r) => l.X + l.Y - r.X + r.Y));
        
        if (insertIndex < 0)
            insertIndex = ~insertIndex;

        _anchors.Insert(insertIndex, anchor);
    }
}
