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
    /// <param name="sprite">The sprite to be drawn for the particle.</param>
    /// <param name="color">The color mask to draw the particle with.</param>
    /// <param name="size">The size of the particle.</param>
    /// <param name="timeToLive">The number of ticks the particle has before expiring.</param>
    public Particle(Sprite sprite, Color color, float size, int timeToLive)
    {
        Require.NotNull(sprite, nameof(sprite));

        Sprite = sprite;
        Color = color;
        Size = size;
        TimeToLive = timeToLive;
    }

    /// <summary>
    /// Gets the sprite to be drawn for the particle.
    /// </summary>
    public Sprite Sprite
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
    /// Gets the number of ticks remaining before the particle expires.
    /// </summary>
    public int TimeToLive
    { get; private set; }

    /// <summary>
    /// Advances the position and lifetime of the particle.
    /// </summary>
    public void Update()
    {
        TimeToLive--;

        Sprite.Update();
    }

    /// <summary>
    /// Draws the particle to the screen.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to use to draw the particle.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        Texture2D texture = Sprite.Texture;

        int width = texture.Width;
        int height = texture.Height;

        var sourceRectangle = new Rectangle(0, 0, width, height);
        var origin = new Vector2((float)width / 2, (float)width / 2);

        spriteBatch.Draw(texture,
                         Sprite.Position,
                         sourceRectangle,
                         Color,
                         Sprite.Angle,
                         origin,
                         Size,
                         SpriteEffects.None,
                         0f);
    }
}
