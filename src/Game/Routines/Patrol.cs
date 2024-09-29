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

using System.ComponentModel;
using BadEcho.Game.AI;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Routines;

/// <summary>
/// Provides a routine that causes an entity to patrol specified positions.
/// </summary>
public sealed class Patrol : Component
{
    private readonly StateMachine<int> _fsm;

    private bool _reversed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Patrol"/> class.
    /// </summary>
    /// <param name="targets">The target positions to move to.</param>
    /// <param name="speed">The permitted positional change per tick.</param>
    public Patrol(ICollection<Vector2> targets, float speed)
        : this(targets, speed, LoopType.Circular)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Patrol"/> class.
    /// </summary>
    /// <param name="targets">The target positions to move to.</param>
    /// <param name="speed">The permitted positional change per tick.</param>
    /// <param name="loopType">
    /// An enumeration value that specifies the type of looping behavior for the patrol sequence.
    /// </param>
    public Patrol(ICollection<Vector2> targets, float speed, LoopType loopType)
    {
        Require.NotNull(targets, nameof(targets));

        int targetIndex = 0;
        var builder = new StateMachineBuilder<int>();

        foreach (Vector2 target in targets)
        {
            int maxIndex = targets.Count - 1;
            bool extremum = targetIndex == 0 || targetIndex == maxIndex;
            int nextIndex = GetNextIndex(targetIndex, maxIndex, loopType);

            var state = builder.Add(targetIndex)
                               .Executes(() => new MoveTo(target, speed))
                               .TransitionsTo(nextIndex)
                                   .WhenComponentsDone()
                                   .WhenTrue(i => !_reversed || i == maxIndex);
            if (extremum)
                state.OnEnter(i => _reversed = i != 0);
            else
            {
                state.TransitionsTo(targetIndex - 1)
                         .WhenComponentsDone()
                         .WhenTrue(_ => _reversed);
            }

            targetIndex++;
        }

        _fsm = builder.Build(0);
    }

    /// <inheritdoc/>
    public override bool Update(IEntity entity, GameUpdateTime time) 
        => _fsm.Update(entity, time);

    private static int GetNextIndex(int currentIndex, int maxIndex, LoopType loopType)
        => loopType switch
        {
            LoopType.None => Math.Min(++currentIndex, maxIndex),
            LoopType.Circular => currentIndex == maxIndex ? 0 : ++currentIndex,
            LoopType.PingPong => currentIndex == maxIndex ? --currentIndex : ++currentIndex,
            _ => throw new InvalidEnumArgumentException(nameof(loopType),
                                                        (int) loopType,
                                                        typeof(LoopType))
        };
}