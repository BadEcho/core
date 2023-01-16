//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game;

/// <summary>
/// Provides a system for the player to exert control over a 2D entity's movement.
/// </summary>
public sealed class PlayerMovementSystem : IMovementSystem
{
    // TODO: These will be set via a hot pluggable config module in future.
    private const Keys MOVEMENT_LEFT = Keys.A;
    private const Keys MOVEMENT_UP = Keys.W;
    private const Keys MOVEMENT_RIGHT = Keys.D;
    private const Keys MOVEMENT_DOWN = Keys.S;
    // TODO: Will be configurable based on entity.
    private const float VELOCITY_INCREMENT = .5f;
    private const float VELOCITY_MAX = 2.0f;
    private const float VELOCITY_MIN = -2.0f;

    /// <inheritdoc/>
    public void UpdateMovement(IPositionalEntity entity)
    {
        Require.NotNull(entity, nameof(entity));

        float xVelocityOriginal = entity.Velocity.X;
        float yVelocityOriginal = entity.Velocity.Y;

        float xVelocity = CalculateUpdatedVelocity(xVelocityOriginal, MOVEMENT_RIGHT, MOVEMENT_LEFT);
        float yVelocity = CalculateUpdatedVelocity(yVelocityOriginal, MOVEMENT_DOWN, MOVEMENT_UP);

        entity.Velocity = new Vector2(xVelocity, yVelocity);
    }

    private static float CalculateUpdatedVelocity(float currentVelocity, Keys positiveDirectionKey, Keys negativeDirectionKey)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        double velocityDelta = 0;

        bool positiveMovement = keyboardState.IsKeyDown(positiveDirectionKey);
        bool negativeMovement = keyboardState.IsKeyDown(negativeDirectionKey);

        if (!positiveMovement && !negativeMovement)
            return 0;

        if (positiveMovement)
        {
            if (currentVelocity < 0)
                currentVelocity = 0;

            velocityDelta += VELOCITY_INCREMENT;
        }

        if (negativeMovement)
        {
            if (currentVelocity > 0)
                currentVelocity = 0;

            velocityDelta -= VELOCITY_INCREMENT;
        }

        return (float) Math.Min(VELOCITY_MAX, Math.Max(VELOCITY_MIN, currentVelocity + velocityDelta));
    }
}
