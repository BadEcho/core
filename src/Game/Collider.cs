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
/// Provides an occupier of space that can collide with other objects.
/// </summary>
public abstract class Collider : ISpatial
{
    private bool _isDirty;

    /// <summary>
    /// Gets or sets a value indicating if the entity should be checked for collisions due to a change
    /// in state.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty || IsAlwaysDirty;
        set => _isDirty = value;
    }

    /// <summary>
    /// Gets or sets the bitmask flag that defines the category this collider belongs to.
    /// </summary>
    public int Category
    { get; set; }

    /// <summary>
    /// Gets or sets the bitmask defining the categories of colliders that can cause collisions
    /// with this collider.
    /// </summary>
    public int CollidableCategories
    { get; set; }
    
    /// <inheritdoc/>
    public abstract IShape Bounds 
    { get; }

    /// <summary>
    /// Gets or sets a value indicating if this collider should always be checked for collisions.
    /// </summary>
    protected bool IsAlwaysDirty
    { get; set; }

    /// <summary>
    /// Returns a value indicating if the provided collider can cause collisions with this collider.
    /// </summary>
    /// <param name="collider">The collider to check for collidability.</param>
    /// <returns>True if <c>collider</c> can collide with this; otherwise, false.</returns>
    public bool CanCollideWith(Collider collider)
    {
        Require.NotNull(collider, nameof(collider));

        return (CollidableCategories & collider.Category) == collider.Category;
    }

    /// <summary>
    /// Resolves a collision that has occurred between this and the provided collider.
    /// </summary>
    /// <param name="collision">The collider the collision has occurred with.</param>
    public abstract bool ResolveCollision(Collider collision);
} 