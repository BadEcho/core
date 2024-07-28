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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a canvas for a texture able to be positioned and moved on the screen in two dimensions.
/// </summary>
public class Sprite : IEntity
{
    private readonly IShape _bounds;

    private EntityCollider _collider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="boundedTexture">The texture of the sprite, with spatial boundaries defined.</param>
    public Sprite(BoundedTexture boundedTexture)
        : this(GetBoundedTexture(boundedTexture))
    {
        _bounds = boundedTexture.Bounds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the sprite.</param>
    public Sprite(Texture2D texture)
    {
        Require.NotNull(texture, nameof(texture));
        
        Texture = texture;
        _bounds = new RectangleF(PointF.Empty, Texture.Bounds.Size);
        Collider = new EntityCollider();
    }

    /// <summary>
    /// Gets the texture of the sprite.
    /// </summary>
    public Texture2D Texture
    { get; }
    
    /// <summary>
    /// Gets or sets the collider that manages collisions for this sprite.
    /// </summary>
    public EntityCollider Collider
    {
        get => _collider;
        [MemberNotNull(nameof(_collider))]
        set
        {
            _collider = value;
            _collider.Entity = this;
        }
    }

    /// <inheritdoc/>
    public ICollection<Component> Components
    { get; } = [];
    
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

    /// <summary>
    /// Executes associated components and advances the movement of the sprite by one tick.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public virtual void Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        foreach (Component component in Components)
        {
            component.Update(this, time);
        }

        Vector2 lastPosition = Position;

        Position += Move.ScaleToTime(Velocity, time);
        Collider.IsDirty = Velocity != Vector2.Zero;
        LastMovement = Position - lastPosition;

        Angle += Move.ScaleToTime(AngularVelocity, time);
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the sprite.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));
        
        spriteBatch.Draw(Texture, (Rectangle) GetTargetArea(), GetSourceArea(), Color.White);
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="ConfiguredSpriteBatch"/> instance to use to draw the sprite.</param>
    public void Draw(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));
        
        spriteBatch.Draw(Texture, (Rectangle) GetTargetArea(), GetSourceArea(), Color.White);
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the sprite.</param>
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
    /// Gets the bounding rectangle of the region of the screen that the sprite's texture will be drawn to.
    /// </summary>
    /// <returns>The bounding rectangle of the region of the screen that <see cref="Texture"/> will be drawn to.</returns>
    protected virtual RectangleF GetTargetArea()
        => new(Position.X, Position.Y, Texture.Width, Texture.Height);

    private static Texture2D GetBoundedTexture(BoundedTexture boundedTexture)
    {
        Require.NotNull(boundedTexture, nameof(boundedTexture));

        return boundedTexture.Texture;
    }
}
