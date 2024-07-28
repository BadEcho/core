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

using System.Diagnostics.CodeAnalysis;
using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a default collider for an entity.
/// </summary>
public class EntityCollider : Collider
{
    /// <inheritdoc/>
    public override IShape Bounds
    {
        get
        {
            EnsureEntitySet();

            return Entity.Bounds;
        }
    }

    /// <summary>
    /// Gets or sets the entity associated with this collider.
    /// </summary>
    public IEntity? Entity
    { get; set; }

    /// <inheritdoc/>
    public override bool ResolveCollision(Collider collision)
    {
        Require.NotNull(collision, nameof(collision));
        EnsureEntitySet();

        Vector2 penetration = Bounds.CalculatePenetration(collision.Bounds);
        Entity.Position += penetration;

        return true;
    }

    [MemberNotNull(nameof(Entity))]
    private void EnsureEntitySet()
    {
        if (Entity == null)
            throw new InvalidOperationException(Strings.NoEntityAssociated);
    }
}
