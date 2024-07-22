//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under a
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides a system that has no effect on a positional entity's movement.
/// </summary>
public sealed class NonMovementSystem : IMovementSystem
{
    /// <inheritdoc />
    public void UpdateMovement(IEntity entity)
    { }

    /// <inheritdoc />
    public void ApplyPenetration(IEntity entity, Vector2 penetration)
    {
        Require.NotNull(entity, nameof(entity));

        entity.Position += penetration;
    }
}
