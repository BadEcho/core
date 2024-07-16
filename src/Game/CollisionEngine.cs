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

using BadEcho.Logging;

namespace BadEcho.Game;

/// <summary>
/// Provides an engine for handling collisions between entities.
/// </summary>
public sealed class CollisionEngine
{
    private readonly List<ISpatialEntity> _collidables = [];
    private readonly Quadtree _collisionTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionEngine"/> class.
    /// </summary>
    /// <param name="bounds">
    /// The bounding rectangle of the region that the collidables handled by this engine occupy.
    /// </param>
    public CollisionEngine(RectangleF bounds)
        => _collisionTree = new Quadtree(bounds);

    /// <summary>
    /// Adds the provided collidable entity into this engine's collision tree.
    /// </summary>
    /// <param name="collidable">The collidable entity to add to this engine's collision tree.</param>
    public void AddCollidable(ISpatialEntity collidable)
    {
        Require.NotNull(collidable, nameof(collidable));

        if (_collidables.Contains(collidable))
            return;

        _collidables.Add(collidable);
        _collisionTree.Insert(collidable);
    }

    /// <summary>
    /// Removes the specified collidable entity from this engine's collision tree.
    /// </summary>
    /// <param name="collidable">The collidable entity to remove from this engine's collision tree.</param>
    public void RemoveCollidable(ISpatialEntity collidable)
    {
        if (!_collidables.Remove(collidable))
            return;
        
        _collisionTree.Remove(collidable);
    }

    public void Clear()
    {
        foreach (ISpatialEntity collidable in _collidables)
        {
            _collisionTree.Remove(collidable);
        }

        _collidables.Clear();
    }

    /// <summary>
    /// Refreshes the collision tree, processing collisions while doing so.
    /// </summary>
    public void Update()
    {
        IEnumerable<ISpatialEntity> collidablesToCheck
            = _collidables.Where(c => c.CheckForCollisions);

        foreach (ISpatialEntity collidable in collidablesToCheck)
        {
            _collisionTree.Remove(collidable);

            IEnumerable<ISpatialEntity> collisions = _collisionTree.FindCollisions(collidable);

            foreach (var collision in collisions)
            {
                if (!collidable.ResolveCollision(collision.Bounds))
                    collision.ResolveCollision(collidable.Bounds);
            }

            _collisionTree.Insert(collidable);
        }
    }
}
