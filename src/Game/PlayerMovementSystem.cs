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
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game;

/// <summary>
/// Provides a system for the player to exert control over an entity's movement.
/// </summary>
public sealed class PlayerMovementSystem : Component
{
    // TODO: These will be set via a hot pluggable config module in the future. This entire class will have a major refactor.
    private const Keys MOVEMENT_LEFT = Keys.A;
    private const Keys MOVEMENT_UP = Keys.W;
    private const Keys MOVEMENT_RIGHT = Keys.D;
    private const Keys MOVEMENT_DOWN = Keys.S;
    // TODO: Will be configurable based on entity.
    private const float VELOCITY_MAX = 50f;
    private const float VELOCITY_MIN = -50f;

    /// <inheritdoc/>
    public override void Update(IEntity entity, GameUpdateTime time)
    {
        Require.NotNull(entity, nameof(entity));

        float xVelocity = CalculateUpdatedVelocity(MOVEMENT_RIGHT, MOVEMENT_LEFT);
        float yVelocity = CalculateUpdatedVelocity(MOVEMENT_DOWN, MOVEMENT_UP);

        entity.Velocity = new Vector2(xVelocity, yVelocity);
    }
    
    private static float CalculateUpdatedVelocity(Keys positiveDirectionKey, Keys negativeDirectionKey)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        bool positiveMovement = keyboardState.IsKeyDown(positiveDirectionKey);
        bool negativeMovement = keyboardState.IsKeyDown(negativeDirectionKey);

        if (!positiveMovement && !negativeMovement)
            return 0;

        return positiveMovement ? VELOCITY_MAX : VELOCITY_MIN;
    }
}
