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

namespace BadEcho.Game;

/// <summary>
/// Provides an engine for handling collisions between colliders.
/// </summary>
public sealed class CollisionEngine
{
    private readonly List<Collider> _colliders = [];
    private readonly Quadtree<Collider> _collisionTree;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionEngine"/> class.
    /// </summary>
    /// <param name="bounds">
    /// The bounding rectangle of the region that the colliders handled by this engine occupy.
    /// </param>
    public CollisionEngine(RectangleF bounds)
        => _collisionTree = new Quadtree<Collider>(bounds) { AllowOutOfBounds = true };

    /// <summary>
    /// Adds the provided collider into this engine's collision tree.
    /// </summary>
    /// <param name="collider">The collider to add to this engine's collision tree.</param>
    public void Register(Collider collider)
    {
        Require.NotNull(collider, nameof(collider));

        if (_colliders.Contains(collider))
            return;

        _colliders.Add(collider);
        _collisionTree.Insert(collider);
    }

    /// <summary>
    /// Removes the specified collider from this engine's collision tree.
    /// </summary>
    /// <param name="collider">The collider to remove from this engine's collision tree.</param>
    public void Unregister(Collider collider)
    {
        if (!_colliders.Remove(collider))
            return;
        
        _collisionTree.Remove(collider);
    }

    /// <summary>
    /// Removes all registered colliders from this engine's collision tree.
    /// </summary>
    public void UnregisterAll()
    {
        foreach (Collider collider in _colliders)
        {
            _collisionTree.Remove(collider);
        }

        _colliders.Clear();
    }

    /// <summary>
    /// Refreshes the collision tree, processing collisions while doing so.
    /// </summary>
    public void Update()
    {
        IEnumerable<Collider> collidersToCheck
            = _colliders.Where(c => c.IsDirty);

        foreach (Collider collider in collidersToCheck)
        {
            _collisionTree.Remove(collider);

            IEnumerable<Collider> collisions = _collisionTree.FindCollisions(collider)
                                                             .Where(collider.CanCollideWith);
            foreach (var collision in collisions)
            {
                if (!collider.ResolveCollision(collision) && collision.CanCollideWith(collider))
                    collision.ResolveCollision(collider);
            }

            _collisionTree.Insert(collider);
        }
    }
}
