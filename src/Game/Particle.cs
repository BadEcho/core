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
/// Provides a minute sprite used by a particle system to reproduce the visuals exhibited by highly chaotic systems, natural phenomena,
/// or processes caused by chemical reactions.
/// </summary>
public sealed class Particle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Particle"/> class.
    /// </summary>
    /// <param name="movingTexture">The moving texture of the particle.</param>
    /// <param name="color">The color mask to draw the particle with.</param>
    /// <param name="size">The size of the particle.</param>
    /// <param name="timeToLive">The number of ticks the sprite has before expiring.</param>
    public Particle(MovingTexture movingTexture, Color color, float size, int timeToLive)
    {
        Require.NotNull(movingTexture, nameof(movingTexture));

        MovingTexture = movingTexture;
        Color = color;
        Size = size;
        TimeToLive = timeToLive;
    }
    
    /// <summary>
    /// Gets the moving particle's texture.
    /// </summary>
    public MovingTexture MovingTexture
    { get; }

    /// <summary>
    /// Gets the color mask the particle is drawn with.
    /// </summary>
    public Color Color
    { get; }

    /// <summary>
    /// Gets the size of the particle.
    /// </summary>
    public float Size
    { get; }

    /// <summary>
    /// Gets the number of ticks remaining before the sprite expires.
    /// </summary>
    public int TimeToLive
    { get; private set; }

    /// <summary>
    /// Advances the position and lifetime of the sprite.
    /// </summary>
    public void Update()
    {
        TimeToLive--;

        MovingTexture.Update();
    }

    /// <summary>
    /// Draws the sprite to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the sprite.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        int width = MovingTexture.Texture.Width;
        int height = MovingTexture.Texture.Height;
        
        var sourceRectangle = new Rectangle(0, 0, width, height);
        var origin = new Vector2((float) width / 2, (float) width / 2);

        spriteBatch.Draw(MovingTexture.Texture,
                         MovingTexture.Position,
                         sourceRectangle,
                         Color,
                         MovingTexture.Angle,
                         origin,
                         Size,
                         SpriteEffects.None,
                         0f);
    }
}
