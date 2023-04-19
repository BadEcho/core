//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using BadEcho.Game.Properties;

namespace BadEcho.Game;

/// <summary>
/// Provides a tree data structure in which each internal (non-leaf) node has exactly four children, useful
/// for 2D spatial information queries.
/// </summary>
public sealed class Quadtree
{
    private readonly List<ISpatialEntity> _elements = new();
    
    private readonly int _bucketCapacity;
    private readonly int _maxDepth;

    private Quadtree? _topLeft, _topRight, _bottomLeft, _bottomRight;

    /// <summary>
    /// Initializes a new instance of the <see cref="Quadtree"/> class.
    /// </summary>
    /// <param name="bounds">The bounding rectangle of the region that this quadtree's contents occupy.</param>
    /// <remarks>
    /// This initializes the <see cref="Quadtree"/> instance with a default bucket capacity of <c>32</c> and a default maximum
    /// node depth of <c>5</c>, which are reasonable general-purpose values for these settings.
    /// </remarks>
    public Quadtree(RectangleF bounds)
        : this(bounds, 32, 5)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Quadtree"/> class.
    /// </summary>
    /// <param name="bounds">The bounding rectangle of the region that this quadtree's contents occupy.</param>
    /// <param name="bucketCapacity">The maximum number of items allowed in this node before it gets quartered.</param>
    /// <param name="maxDepth">The maximum number of levels of child nodes allowed across the entire quad tree.</param>
    public Quadtree(RectangleF bounds, int bucketCapacity, int maxDepth)
    {
        _bucketCapacity = bucketCapacity;
        _maxDepth = maxDepth;
        
        Bounds = bounds;
    }

    /// <summary>
    /// Gets the bounding rectangle of the region that this quadtree's contents occupy.
    /// </summary>
    public RectangleF Bounds
    { get; }

    /// <summary>
    /// Gets the level of this node within the overall quadtree; that is, the number of edges on the path from this node to the
    /// root node.
    /// </summary>
    public int Level
    { get; init; }

    /// <summary>
    /// Gets a value indicating if this node is a terminal node for the tree (i.e., it has no children).
    /// </summary>
    /// <remarks>
    /// The majority of spatial elements reside in leaf nodes, the only exceptions being elements whose bounds overlap multiple
    /// split node boundaries.
    /// </remarks>
    [MemberNotNullWhen(false, nameof(_topLeft), nameof(_topRight), nameof(_bottomLeft), nameof(_bottomRight))]
    public bool IsLeaf
        => _topLeft == null || _topRight == null || _bottomLeft == null || _bottomRight == null;
    
    /// <summary>
    /// Inserts the specified spatial element into the quadtree at the appropriate node.
    /// </summary>
    /// <param name="element">The spatial element to insert into the quadtree.</param>
    public void Insert(ISpatialEntity element)
    {
        Require.NotNull(element, nameof(element));

        if (!Bounds.Contains(element.Bounds))
            throw new ArgumentException(Strings.ElementOutsideQuadtreeBounds, nameof(element));

        // A node exceeding its allotted number of items will get split (if it hasn't been already) into four equal quadrants.
        if (_elements.Count >= _bucketCapacity)
            Split();

        Quadtree? containingChild = GetContainingChild(element.Bounds);

        if (containingChild != null)
        {
            containingChild.Insert(element);
        }
        else
        {   // If no child was returned, then this is either a leaf node, or the element's boundaries overlap multiple children.
            // Either way, the element gets assigned to this node.
            _elements.Add(element);
        }
    }

    /// <summary>
    /// Removes the specified spatial element from the quadtree and whichever node it's been assigned to.
    /// </summary>
    /// <param name="element">The spatial element to remove from the quadtree.</param>
    /// <returns>True if <c>element</c> is successfully removed; otherwise, false.</returns>
    public bool Remove(ISpatialEntity element)
    {
        Require.NotNull(element, nameof(element));

        Quadtree? containingChild = GetContainingChild(element.Bounds);

        // If no child was returned, then this is the leaf node (or potentially non-leaf node, if the element's boundaries overlap
        // multiple children) containing the element.
        bool removed = containingChild?.Remove(element) ?? _elements.Remove(element);
        
        // If the total descendant element count is less than the bucket capacity, we ensure the node is in a non-split state.
        if (removed && CountElements() <= _bucketCapacity)
            Merge();

        return removed;
    }

    /// <summary>
    /// Looks for and returns all spatial elements that exist within this node and its children whose bounds collide with
    /// the specified spatial element.
    /// </summary>
    /// <param name="element">The spatial element to find collisions for.</param>
    /// <returns>All spatial elements that collide with <c>element</c>.</returns>
    public IEnumerable<ISpatialEntity> FindCollisions(ISpatialEntity element)
    {
        Require.NotNull(element, nameof(element));

        var nodes = new Queue<Quadtree>();
        var collisions = new List<ISpatialEntity>();

        nodes.Enqueue(this);

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();

            if (!element.Bounds.Intersects(node.Bounds))
                continue;

            collisions.AddRange(node._elements.Where(e => e.Bounds.Intersects(element.Bounds)));

            if (!node.IsLeaf)
            {
                if (element.Bounds.Intersects(node._topLeft.Bounds))
                    nodes.Enqueue(node._topLeft);

                if (element.Bounds.Intersects(node._topRight.Bounds))
                    nodes.Enqueue(node._topRight);

                if (element.Bounds.Intersects(node._bottomLeft.Bounds))
                    nodes.Enqueue(node._bottomLeft);

                if (element.Bounds.Intersects(node._bottomRight.Bounds))
                    nodes.Enqueue(node._bottomRight);
            }
        }

        return collisions;
    }

    /// <summary>
    /// Gets the total number of elements belonging to this and all descending nodes.
    /// </summary>
    /// <returns>The total number of elements belong to this and all descending nodes.</returns>
    public int CountElements() 
    {
        int count = _elements.Count;

        if (!IsLeaf)
        {
            count += _topLeft.CountElements();
            count += _topRight.CountElements();
            count += _bottomLeft.CountElements();
            count += _bottomRight.CountElements();
        }

        return count;
    }

    /// <summary>
    /// Retrieves the elements belonging to this and all descendant nodes.
    /// </summary>
    /// <returns>A sequence of the elements belonging to this and all descendant nodes.</returns>
    public IEnumerable<ISpatialEntity> GetElements()
    {
        var children = new List<ISpatialEntity>();
        var nodes = new Queue<Quadtree>();

        nodes.Enqueue(this);

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();

            if (!node.IsLeaf)
            {
                nodes.Enqueue(node._topLeft);
                nodes.Enqueue(node._topRight);
                nodes.Enqueue(node._bottomLeft);
                nodes.Enqueue(node._bottomRight);
            }

            children.AddRange(node._elements);
        }

        return children;
    }

    private void Split()
    {   // If we're not a leaf node, then we're already split.
        if (!IsLeaf)
            return;

        // Splitting is only allowed if it doesn't cause us to exceed our maximum depth.
        if (Level + 1 > _maxDepth)
            return;

        _topLeft = CreateChild(Bounds.Location);
        _topRight = CreateChild(new PointF(Bounds.Center.X, Bounds.Location.Y));
        _bottomLeft = CreateChild(new PointF(Bounds.Location.X, Bounds.Center.Y));
        _bottomRight = CreateChild(new PointF(Bounds.Center.X, Bounds.Center.Y));

        var elements = _elements.ToList();

        foreach (var element in elements)
        {
            Quadtree? containingChild = GetContainingChild(element.Bounds);
            // An element is only moved if it completely fits into a child quadrant.
            if (containingChild != null)
            {   
                _elements.Remove(element);

                containingChild.Insert(element);
            }
        }
    }

    private Quadtree CreateChild(PointF location)
        => new(new RectangleF(location, Bounds.Size / 2), _bucketCapacity, _maxDepth)
           {
               Level = Level + 1
           };

    private void Merge()
    {   // If we're a leaf node, then there is nothing to merge.
        if (IsLeaf)
            return;

        _elements.AddRange(_topLeft._elements);
        _elements.AddRange(_topRight._elements);
        _elements.AddRange(_bottomLeft._elements);
        _elements.AddRange(_bottomRight._elements);

        _topLeft = _topRight = _bottomLeft = _bottomRight = null;
    }

    private Quadtree? GetContainingChild(IShape bounds)
    {
        if (IsLeaf)
            return null;

        if (_topLeft.Bounds.Contains(bounds))
            return _topLeft;
        
        if (_topRight.Bounds.Contains(bounds))
            return _topRight;
        
        if (_bottomLeft.Bounds.Contains(bounds))
            return _bottomLeft;
        
        return _bottomRight.Bounds.Contains(bounds) ? _bottomRight : null;
    }
}
