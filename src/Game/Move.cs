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

namespace BadEcho.Game;

/// <summary>
/// Provides a set of static methods that aid in movement.
/// </summary>
public static class Move
{
    /// <summary>
    /// Calculates the movement velocity to apply to a base position to approach a target.
    /// </summary>
    /// <param name="position">The base position being moved from.</param>
    /// <param name="target">The target position to move to.</param>
    /// <param name="maxChange">The maximum change to <c>position</c> permitted.</param>
    /// <param name="time">The game timing configuration and state.</param>
    /// <returns>A movement velocity that can be applied to <c>position</c> to approach <c>target</c>.</returns>
    public static Vector2 Approach(Vector2 position, Vector2 target, float maxChange, GameUpdateTime time)
    {
        if (maxChange == 0f || position == target)
            return Vector2.Zero;

        Vector2 difference = target - position;
        Vector2 velocity= difference.Normalized() * maxChange;
        
        float differenceLength = difference.Length();
        float scaledVelocityLength =  ScaleToTime(velocity, time).Length();

        // If the rate of movement, scaled by time, exceeds the distance then we need to cap it so that we don't overshoot.
        if (scaledVelocityLength > differenceLength)
            velocity *= (differenceLength / scaledVelocityLength);

        return velocity;
    }

    /// <summary>
    /// Scales movement velocity so that it is proportionate to the amount of time that has elapsed since a
    /// previous update.
    /// </summary>
    /// <param name="velocity">The movement velocity to scale.</param>
    /// <param name="time">The game timing configuration and state.</param>
    /// <returns><c>velocity</c> scaled based on <c>time</c>.</returns>
    public static Vector2 ScaleToTime(Vector2 velocity, GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        return velocity * (float) time.ElapsedGameTime.TotalSeconds;
    }

    /// <summary>
    /// Scales movement speed so that it is proportionate to the amount of time that has elapsed since a
    /// previous update.
    /// </summary>
    /// <param name="speed">The movement speed to scale.</param>
    /// <param name="time">The game timing configuration and state.</param>
    /// <returns><c>speed</c> scaled based on <c>time</c>.</returns>
    public static float ScaleToTime(float speed, GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        return speed * (float) time.ElapsedGameTime.TotalSeconds;
    }
}
 