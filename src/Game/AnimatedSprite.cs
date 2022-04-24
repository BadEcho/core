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
/// Provides a canvas for a texture containing multiple smaller images arranged tabularly, allowing for the animation of a 2D entity
/// via selective image drawing of the frames found on a provided sprite sheet.
/// </summary>
public sealed class AnimatedSprite : Sprite
{
    private readonly IMovementSystem _movementSystem;
    private readonly SpriteSheet _sheet;
    private int _currentFrame;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
    /// </summary>
    /// <param name="sheet">The sprite sheet containing the various animation frames of the sprite.</param>
    /// <param name="position">The drawing location of the animated sprite.</param>
    /// <param name="movementSystem"></param>
    public AnimatedSprite(SpriteSheet sheet, Vector2 position, IMovementSystem movementSystem)
        : base(GetSheetTexture(sheet), position)
    {
        Require.NotNull(movementSystem, nameof(movementSystem));
        
        _sheet = sheet;
        _movementSystem = movementSystem;
    }
     
    /// <inheritdoc/>
    public override void Update()
    {
        base.Update();

        _movementSystem.UpdateMovement(this);

        if (Velocity != Vector2.Zero)
            _currentFrame++;

        if (_currentFrame == _sheet.TotalFrames)
            _currentFrame = 0;
    }

    /// <inheritdoc/>
    protected override Rectangle GetSourceRectangle() 
        => _sheet.GetFrameRectangle(_currentFrame);

    /// <inheritdoc/>
    protected override Rectangle GetTargetRectangle()
        => new(Position.ToPoint(), _sheet.FrameSize);

    private static Texture2D GetSheetTexture(SpriteSheet sheet)
    {
        Require.NotNull(sheet, nameof(sheet));

        return sheet.Texture;
    }
}
