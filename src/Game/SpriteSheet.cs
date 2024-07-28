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
/// Because a sprite sheet contains uniformly sized images arranged as a grid, an image can be loaded simply by specifying a row
/// and column, allowing for simple animation frame advancement.
/// </para>
/// </remarks>
public sealed class SpriteSheet
{
    private readonly Dictionary<string, SpriteAnimationSequence> _nameAnimationMap
        = new(StringComparer.OrdinalIgnoreCase);

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
    /// Registers an animation with the sprite sheet using the provided animation sequence.
    /// </summary>
    /// <param name="sequence">The frames in an animation sequence for a sprite.</param>
    /// <exception cref="ArgumentException"><c>name</c> is already associated with an existing registered animation.</exception>
    public void AddAnimation(SpriteAnimationSequence sequence)
    {
        Require.NotNull(sequence, nameof(sequence));

        if (!_nameAnimationMap.TryAdd(sequence.Name, sequence))
            throw new ArgumentException(Strings.SheetAlreadyHasAnimation, nameof(sequence));
    }

    /// <summary>
    /// Gets the named sprite animation.
    /// </summary>
    /// <param name="name">The name of the sprite animation.</param>
    /// <returns>The animation for <c>name</c>.</returns>
    public SpriteAnimation GetAnimation(string name)
    {
        if (!_nameAnimationMap.TryGetValue(name, out SpriteAnimationSequence? sequence))
            throw new ArgumentException(Strings.SheetNoFramesForAnimation, nameof(name));

        var frames = Enumerable.Repeat(sequence.Duration,
                                       sequence.EndFrame - sequence.StartFrame + 1);

        return new SpriteAnimation(frames, sequence.Name);
    }

    /// <summary>
    /// Gets the region of the sprite sheet's texture corresponding to the requested frame of the specified animation.
    /// </summary>
    /// <param name="animation">The animation containing the frame that the region will encompass.</param>
    /// <returns>
    /// The bounding rectangle of the region of <see cref="Texture"/> that encompasses the current frame for
    /// <c>animation</c>.
    /// </returns>
    /// <exception cref="ArgumentException">No frames for <c>animation</c> were found.</exception>
    public Rectangle GetFrameRectangle(SpriteAnimation animation)
    {
        Require.NotNull(animation, nameof(animation));
        
        if (!_nameAnimationMap.TryGetValue(animation.Name, out SpriteAnimationSequence? sequence))
            throw new ArgumentException(Strings.SheetNoFramesForAnimation, nameof(animation));

        int frameIndex = animation.CurrentFrame + sequence.StartFrame;
        Size frameLocation = new(frameIndex % ColumnCount, frameIndex / ColumnCount);

        return new Rectangle(frameLocation * FrameSize, FrameSize);
    }
}
