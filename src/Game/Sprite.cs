//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a canvas for a texture able to be positioned on the screen as a 2D entity.
/// </summary>
public sealed class Sprite
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the sprite.</param>
    /// <param name="position">The drawing location of the sprite.</param>
    public Sprite(Texture2D texture, Vector2 position)
        : this(texture, position, Vector2.Zero)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>/
    /// <param name="texture">The texture of the sprite.</param>
    /// <param name="initialPosition">The initial drawing location of the sprite.</param>
    /// <param name="velocity">The rate of change of the sprite's position.</param>
    public Sprite(Texture2D texture, Vector2 initialPosition, Vector2 velocity)
        : this(texture, initialPosition, velocity, 0, 0)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the sprite.</param>
    /// <param name="initialPosition">The initial drawing location of the sprite.</param>
    /// <param name="velocity">The rate of change of the sprite's position.</param>
    /// <param name="initialAngle">The amount that the sprite is initially being rotated about its point of rotation.</param>
    /// <param name="angularVelocity">The rate of change of the sprite's angle.</param>
    public Sprite(Texture2D texture, Vector2 initialPosition, Vector2 velocity, float initialAngle, float angularVelocity)
    {
        Require.NotNull(texture, nameof(texture));

        Texture = texture;
        Position = initialPosition;
        Velocity = velocity;
        Angle = initialAngle;
        AngularVelocity = angularVelocity;
    }

    /// <summary>
    /// Gets the texture of the sprite.
    /// </summary>
    public Texture2D Texture
    { get; }

    /// <summary>
    /// Gets the current drawing location of the sprite.
    /// </summary>
    public Vector2 Position
    { get; private set; }

    /// <summary>
    /// Gets the rate of change of the sprite's position.
    /// </summary>
    public Vector2 Velocity
    { get; }

    /// <summary>
    /// Gets the amount that the sprite is currently being rotated about its point of rotation.
    /// </summary>
    public float Angle
    { get; private set; }

    /// <summary>
    /// Gets the rate of change of the sprite's angle.
    /// </summary>
    public float AngularVelocity
    { get; }

    /// <summary>
    /// Advances the movement of the sprite by one tick.
    /// </summary>
    public void Update()
    {
        Position += Velocity;
        Angle += AngularVelocity;
    }
}
