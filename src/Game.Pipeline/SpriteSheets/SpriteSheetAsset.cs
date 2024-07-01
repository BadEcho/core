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

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides configuration data for a sprite sheet asset.
/// </summary>
public sealed class SpriteSheetAsset
{
    /// <summary>
    /// Gets or sets the path to the file containing the texture of the individual animation frames that compose the sprite sheet.
    /// </summary>
    public string TexturePath 
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets the number of rows of frames in the sprite sheet.
    /// </summary>
    public int RowCount 
    { get; init; }

    /// <summary>
    /// Gets the number of columns of frames in the sprite sheet.
    /// </summary>
    public int ColumnCount 
    { get; init; }
    
    /// <summary>
    /// Gets the index of the frame to initially draw prior to any animations starting.
    /// </summary>
    public int InitialFrame
    { get; init; }

    /// <summary>
    /// Gets the animation sequences the sprite sheet contains.
    /// </summary>
    public IReadOnlyCollection<SpriteAnimationSequence> Animations
    { get; init; } = new List<SpriteAnimationSequence>();
}
