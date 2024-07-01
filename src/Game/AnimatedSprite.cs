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
/// Provides a canvas for a texture containing multiple smaller images arranged tabularly, allowing for the animation of an entity
/// via selective image drawing of the frames found on a provided sprite sheet.
/// </summary>
public sealed class AnimatedSprite : Sprite
{
    private readonly SpriteSheet _sheet;
    private SpriteAnimation _currentAnimation;
    private MovementDirection _currentDirection; 

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
    /// </summary>
    /// <param name="sheet">The sprite sheet containing the various animation frames of the sprite.</param>
    /// <param name="movementSystem">The movement system controlling the sprite's movement.</param>
    public AnimatedSprite(SpriteSheet sheet, IMovementSystem movementSystem)
        : base(GetSheetTexture(sheet), movementSystem)
    {
        _sheet = sheet;
        _currentAnimation = sheet.GetAnimation(string.Empty);
    }

    /// <inheritdoc/>
    public override void Update(GameUpdateTime time)
    {
        base.Update(time);

        MovementDirection newDirection = Velocity.ToDirection();

        if (_currentDirection != newDirection)
        {
            if (newDirection == MovementDirection.None)
                _currentAnimation.Pause();
            else
            {
                _currentAnimation.Stop();
                _currentAnimation = _sheet.GetAnimation(newDirection.ToString());
                _currentAnimation.Play();
            }

            _currentDirection = newDirection;
        }

        _currentAnimation.Update(time);
    }

    /// <inheritdoc/>
    protected override Rectangle GetSourceArea() 
        => _sheet.GetFrameRectangle(_currentAnimation);

    /// <inheritdoc/>
    protected override RectangleF GetTargetArea()
        => new(Position, _sheet.FrameSize);

    private static BoundedTexture GetSheetTexture(SpriteSheet sheet)
    {
        Require.NotNull(sheet, nameof(sheet));

        return new BoundedTexture(new RectangleF(PointF.Empty, sheet.FrameSize), sheet.Texture);
    }
}
