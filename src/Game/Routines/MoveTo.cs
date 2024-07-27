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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Routines;

/// <summary>
/// Provides a routine that moves an entity to a specified position.
/// </summary>
public sealed class MoveTo : Component
{
    private readonly Vector2 _target;
    private readonly float _maxSpeed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveTo"/> class.
    /// </summary>
    /// <param name="target">The target position to move to.</param>
    /// <param name="maxSpeed">The maximum speed to move at.</param>
    public MoveTo(Vector2 target, float maxSpeed)
    {
        _target = target;
        _maxSpeed = maxSpeed;
    }

    /// <inheritdoc/>
    public override void Update(IEntity entity, GameUpdateTime time)
    {
        Require.NotNull(entity, nameof(entity));

        entity.Velocity = Move.Approach(entity.Position, _target, _maxSpeed, time);
    }
}