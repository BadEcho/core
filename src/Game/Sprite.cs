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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a canvas for a texture able to be positioned on the screen as a 2D entity.
/// </summary>
public class Sprite : IPositionalEntity, ISpatialEntity
{
    private readonly IMovementSystem _movementSystem;
    private readonly IShape _bounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="boundedTexture">The texture of the sprite, with spatial boundaries defined.</param>
    public Sprite(BoundedTexture boundedTexture)
        : this(boundedTexture, new NonMovementSystem())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="boundedTexture">The texture of the sprite, with spatial boundaries defined.</param>
    /// <param name="movementSystem">The movement system controlling the sprite's movement.</param>
    public Sprite(BoundedTexture boundedTexture, IMovementSystem movementSystem)
        : this(GetBoundedTexture(boundedTexture), movementSystem)
    {
        _bounds = boundedTexture.Bounds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the sprite.</param>
    public Sprite(Texture2D texture)
        : this(texture, new NonMovementSystem())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the sprite.</param>
    /// <param name="movementSystem">The movement system controlling the sprite's movement.</param>
    public Sprite(Texture2D texture, IMovementSystem movementSystem)
    {
        Require.NotNull(texture, nameof(texture));
        Require.NotNull(movementSystem, nameof(movementSystem));
        
        Texture = texture;
        _bounds = new RectangleF(PointF.Empty, Texture.Bounds.Size);
        _movementSystem = movementSystem;
    }

    /// <summary>
    /// Gets the texture of the sprite.
    /// </summary>
    public Texture2D Texture
    { get; }

    /// <inheritdoc/>
    public Vector2 Position
    { get; set; }

    /// <inheritdoc/>
    public Vector2 LastMovement
    { get; private set; }

    /// <inheritdoc/>
    public Vector2 Velocity
    { get; set; }

    /// <inheritdoc/>
    public float Angle
    { get; set; }

    /// <inheritdoc/>
    public float AngularVelocity
    { get; set; }

    /// <inheritdoc />
    public IShape Bounds 
        => _bounds.CenterAt(GetTargetArea().Center);

    /// <inheritdoc />
    public void ResolveCollision(IShape shape)
    {
        Vector2 penetration = Bounds.CalculatePenetration(shape);

        _movementSystem.ApplyPenetration(this, penetration);
    }

    /// <summary>
    /// Advances the movement of the sprite by one tick.
    /// </summary>
    /// <param name="state">The state of the game at this given point in time.</param>
    public virtual void Update(GameState state)
    {
        Require.NotNull(state, nameof(state));

        _movementSystem.UpdateMovement(this);

        float timeScale 
            = (float) (state.Time.ElapsedGameTime.TotalMilliseconds / state.TargetElapsedTime.TotalMilliseconds);
        
        Vector2 lastPosition = Position;
        
        Position += Vector2.Multiply(Velocity, timeScale);
        LastMovement = Position - lastPosition;

        Angle += AngularVelocity * timeScale;
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the sprite with.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));
        
        spriteBatch.Draw(Texture, (Rectangle) GetTargetArea(), GetSourceArea(), Color.White);
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the sprite with.</param>
    /// <param name="color">The color mask the sprite is drawn with.</param>
    /// <param name="size">The size of the sprite.</param>
    public void Draw(SpriteBatch spriteBatch, Color color, float size)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        var origin = new Vector2((float)Texture.Width / 2, (float)Texture.Height / 2);

        spriteBatch.Draw(Texture,
                         Position,
                         GetSourceArea(),
                         color,
                         Angle,
                         origin,
                         size,
                         SpriteEffects.None,
                         0f);
    }

    /// <summary>
    /// Gets the bounding rectangle of the region of the sprite's texture that will be rendered.
    /// </summary>
    /// <returns>The bounding rectangle of the region of <see cref="Texture"/> that will be rendered.</returns>
    protected virtual Rectangle GetSourceArea()
        => new(0, 0, Texture.Width, Texture.Height);

    /// <summary>
    /// Gets the bounding rectangle of the region of the screen that the sprite's texture will be drawn on.
    /// </summary>
    /// <returns>The bounding rectangle of the region of the screen that <see cref="Texture"/> will be drawn on.</returns>
    protected virtual RectangleF GetTargetArea()
        => new(Position.X, Position.Y, Texture.Width, Texture.Height);

    private static Texture2D GetBoundedTexture(BoundedTexture boundedTexture)
    {
        Require.NotNull(boundedTexture, nameof(boundedTexture));

        return boundedTexture.Texture;
    }
}
