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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a texture containing multiple smaller images (frames) arranged in a tabular fashion, allowing for the selective drawing of
/// related images without the need to load and unload any additional textures.
/// </summary>
public sealed class SpriteSheet
{
    private readonly int _rows;
    private readonly int _columns;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheet"/> class.
    /// </summary>
    /// <param name="texture">The texture containing the individual frames that compose the sprite sheet.</param>
    /// <param name="rows">The number of rows containing images in this sprite sheet.</param>
    /// <param name="columns">The number of columns containing images in this sprite sheet.</param>
    public SpriteSheet(Texture2D texture, int rows, int columns)
    {
        Require.NotNull(texture, nameof(texture));

        Texture = texture;

        _rows = rows;
        _columns = columns;
    }

    /// <summary>
    /// Gets the texture containing the individual frames that compose the sprite sheet.
    /// </summary>
    public Texture2D Texture
    { get; }

    /// <summary>
    /// Gets the size of an individual frame in the sprite sheet.
    /// </summary>
    public Point FrameSize 
        => new(Texture.Width / _columns, Texture.Height / _rows);

    /// <summary>
    /// Gets the total number of frames found in the sprite sheet.
    /// </summary>
    public int TotalFrames
        => _columns * _rows;

    /// <summary>
    /// Gets the region of the sprite sheet's texture corresponding to the requested frame.
    /// </summary>
    /// <param name="frame">The specific frame the returned region will encompass on the sprite sheet's texture.</param>
    /// <returns>The bounding rectangle of the region of <see cref="Texture"/> encompassing <c>frame</c>.</returns>
    /// <exception cref="ArgumentException"><c>frame</c> exceeds the total number of frames found in the sprite sheet.</exception>
    public Rectangle GetFrameRectangle(int frame)
    {
        if (frame > TotalFrames)
            throw new ArgumentException(Strings.FrameExceedsSheetTotal, nameof(frame));

        int column = frame % _columns;
        int row = (int) ((float) frame / _columns);

        Point frameLocation = new(column, row);

        return new Rectangle(frameLocation * FrameSize, FrameSize);
    }
}
