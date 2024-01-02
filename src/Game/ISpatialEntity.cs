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
/// Defines an entity that occupies space.
/// </summary>
public interface ISpatialEntity
{
    /// <summary>
    /// Gets the spatial shape of the entity.
    /// </summary>
    /// <remarks>
    /// The spatial shape of the entity acts as its boundary that, if crossed by another entity, would
    /// result in a collision between the two.
    /// </remarks>
    IShape Bounds { get; }

    /// <summary>
    /// Resolves a collision that has occurred with the entity and the provided collision boundary.
    /// </summary>
    /// <param name="shape">The boundary of the colliding entity.</param>
    void ResolveCollision(IShape shape);
}
