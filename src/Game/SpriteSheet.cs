//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a texture containing multiple smaller images (frames) arranged in a tabular fashion, allowing for the selective
/// drawing of said images without the need to load and unload any additional textures.
/// </summary>
/// <remarks>
/// <para>
/// The terms "sprite sheet" and "texture atlas" are often used interchangeably; however, as far as the Bad Echo game framework
/// is concerned: they are two different things. A sprite sheet contains images arranged uniformly and tabularly, all related to
/// a single entity (such as an NPC sprite). A texture atlas contains images optimally packed together as close as possible, all
/// not necessarily corresponding to a single entity (such as different elements on a HUD interface).
/// </para>
/// <para>
/// Because a sprite sheet contains uniformly sized images arranged as a grid, an image can be loaded simply specifying a row and column
/// indices, allowing for the simple advancement of frames its row and column
/// index, moving between frames is as simple as advancing
/// the current frame's position by 
/// </para>
/// </remarks>
public sealed class SpriteSheet
{
    private readonly Dictionary<MovementDirection, int> _directionRows = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheet"/> class.
    /// </summary>
    /// <param name="texture">The texture containing the individual frames that compose the sprite sheet.</param>
    /// <param name="columnCount">The number of columns of frames in this sprite sheet.</param>
    /// <param name="rowCount">The number of rows of frames in this sprite sheet.</param>
    public SpriteSheet(Texture2D texture, int columnCount, int rowCount)
    {
        Require.NotNull(texture, nameof(texture));

        Texture = texture;
        ColumnCount = columnCount;
        RowCount = rowCount;
    }

    /// <summary>
    /// Gets the texture containing the individual frames that compose the sprite sheet.
    /// </summary>
    public Texture2D Texture
    { get; }
    
    /// <summary>
    /// Gets the size of an individual frame in the sprite sheet.
    /// </summary>
    public Size FrameSize
        => new(Texture.Width / ColumnCount, Texture.Height / RowCount);

    /// <summary>
    /// Gets the number of columns of frames in this sprite sheet.
    /// </summary>
    public int ColumnCount
    { get; }

    /// <summary>
    /// Gets the number of rows of frames in this sprite sheet.
    /// </summary>
    public int RowCount 
    { get; }

    /// <summary>
    /// Registers the specified direction of movement and the row in the sprite sheet containing its corresponding frames.
    /// </summary>
    /// <param name="direction">The direction of movement whose frames are being registered.</param>
    /// <param name="row">The index of the row in the sprite sheet containing the movement direction's frames.</param>
    /// <exception cref="ArgumentException">Frames for the <c>direction</c> have already been registered.</exception>
    /// <exception cref="ArgumentException"><c>row</c> is equal to or greater than <see cref="RowCount"/>.</exception>
    public void AddDirection(MovementDirection direction, int row)
    {
        if (_directionRows.ContainsKey(direction))
            throw new ArgumentException(Strings.SheetAlreadyHasDirection, nameof(direction));

        if (row >= RowCount)
            throw new ArgumentException(Strings.SheetDirectionRowOutOfRange, nameof(row));

        _directionRows.Add(direction, row);
    }

    /// <summary>
    /// Gets the region of the sprite sheet's texture corresponding to the requested frame for the specified direction of movement.
    /// </summary>
    /// <param name="direction">The direction of movement the returned region will encompass.</param>
    /// <param name="frame">
    /// The specific frame the returned region will encompass on the sprite sheet's texture for the given direction of movement.
    /// </param>
    /// <returns>The bounding rectangle of the region of <see cref="Texture"/> that encompasses the <c>frame</c> for <c>direction</c>.</returns>
    /// <exception cref="ArgumentException">No frames for the <c>direction</c> were found.</exception>
    /// <exception cref="ArgumentException"><c>frame</c> exceeds the total number of frames found for the <c>direction</c>.</exception>
    public Rectangle GetFrameRectangle(MovementDirection direction, int frame)
    {
        if (!_directionRows.ContainsKey(direction))
            throw new ArgumentException(Strings.SheetNoFramesForDirection, nameof(direction));

        if (frame >= ColumnCount)
            throw new ArgumentException(Strings.SheetFrameExceedsTotal, nameof(frame));

        Size frameLocation = new(frame, _directionRows[direction]);

        return new Rectangle(frameLocation * FrameSize, FrameSize);
    }
}
