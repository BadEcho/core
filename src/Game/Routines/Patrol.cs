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

using BadEcho.Game.AI;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Routines;

/// <summary>
/// Provides a routine that has an entity patrol specified positions.
/// </summary>
public sealed class Patrol : Component
{
    private readonly StateMachine<int> _fsm;

    /// <summary>
    /// Initializes a new instance of the <see cref="Patrol"/> class.
    /// </summary>
    /// <param name="targets">The target positions to move to.</param>
    /// <param name="speed">The permitted positional change per tick.</param>
    public Patrol(ICollection<Vector2> targets, float speed)
    {
        Require.NotNull(targets, nameof(targets));

        int targetIndex = 0;
        var builder = new StateMachineBuilder<int>();

        foreach (Vector2 target in targets)
        {
            int nextIndex = targetIndex + 1 == targets.Count
                ? 0
                : targetIndex + 1;

            builder.Add(targetIndex++)
                   .Executes(() => new MoveTo(target, speed))
                   .TransitionsTo(nextIndex)
                   .WhenComponentsDone();
        }

        _fsm = builder.Build(0);
    }

    /// <inheritdoc/>
    public override bool Update(IEntity entity, GameUpdateTime time) 
        => _fsm.Update(entity, time);
}
