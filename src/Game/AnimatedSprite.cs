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
/// via selective image drawing based on a frame's associated row and column.
/// </summary>
public sealed class AnimatedSprite : Sprite
{
    private readonly int _totalFrames;

    private int _currentFrame;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
    /// </summary>
    /// <param name="texture">The texture of the animated sprite.</param>
    /// <param name="position">The drawing location of the animated sprite.</param>
    /// <param name="rows">The number of rows containing images in the sprite's texture atlas.</param>
    /// <param name="columns">The number of columns containing images in the sprite's texture atlas.</param>

    public AnimatedSprite(Texture2D texture, Vector2 position, int rows, int columns)
        : base(texture, position)
    {
        Rows = rows;
        Columns = columns;

        _totalFrames = Rows * Columns;
    }

    /// <summary>
    /// Gets the number of rows containing images in this sprite's texture atlas.
    /// </summary>
    public int Rows
    { get; }

    /// <summary>
    /// Gets the number of columns containing images in this sprite's texture atlas.
    /// </summary>
    public int Columns
    { get; }

    /// <summary>
    /// The width of each frame's image.
    /// </summary>
    private int FrameWidth
        => Texture.Width / Columns;

    /// <summary>
    /// The height of each frame's image.
    /// </summary>
    private int FrameHeight
        => Texture.Height / Rows;

    /// <inheritdoc/>
    public override void Update()
    {
        base.Update();

        _currentFrame++;

        if (_currentFrame == _totalFrames)
            _currentFrame = 0;
    }

    /// <inheritdoc/>
    protected override Rectangle GetSourceRectangle()
    {
        int row = (int) ((float) _currentFrame / Columns);
        int column = _currentFrame % Columns;

        return new Rectangle(FrameWidth * column, FrameHeight * row, FrameWidth, FrameHeight);
    }

    /// <inheritdoc/>
    protected override Rectangle GetTargetRectangle()
        => new((int) Position.X, (int) Position.Y, FrameWidth, FrameHeight);
}
