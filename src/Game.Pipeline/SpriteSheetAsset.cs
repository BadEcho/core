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

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides configuration data for a sprite sheet asset.
/// </summary>
public sealed class SpriteSheetAsset
{
    /// <summary>
    /// Gets the image format of the texture containing the individual sprite sheet frames, identified here using the format's associated
    /// file extension.
    /// </summary>
    public string TextureFormat 
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of rows of frames in the sprite sheet.
    /// </summary>
    public int Rows 
    { get; init; }

    /// <summary>
    /// Gets the number of columns of frames in the sprite sheet.
    /// </summary>
    public int Columns 
    { get; init; }

    /// <summary>
    /// Gets the row for upward movement.
    /// </summary>
    public int RowUp
    { get; init; }

    /// <summary>
    /// Gets the row for downward movement.
    /// </summary>
    public int RowDown
    { get; init; }

    /// <summary>
    /// Gets the row for leftward movement.
    /// </summary>
    public int RowLeft
    { get; init; }

    /// <summary>
    /// Gets the row for rightward movement.
    /// </summary>
    public int RowRight
    { get; init; }

    /// <summary>
    /// Gets or sets the row containing initially drawn frames, prior to any movement occurring.
    /// </summary>
    public int RowInitial
    { get; set; }
}
