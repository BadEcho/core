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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides configuration for a tile belonging to a tile set.
/// </summary>
public sealed class TileData : Extensible
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileData"/> class.
    /// </summary>
    /// <param name="id">The local identifier of this tile within its tile set.</param>
    /// <param name="sourceArea">
    /// The explicit bounding rectangle of the region of the texture associated with this tile that will be rendered
    /// when drawing this tile, if one exists.
    /// </param>
    /// <param name="animationFrames">The frames in this tile's animation, if any.</param>
    /// <param name="customProperties">The tile's custom properties.</param>
    public TileData(int id, 
                    Rectangle? sourceArea,
                    IEnumerable<TileAnimationFrame> animationFrames,
                    CustomProperties customProperties)
        : base(customProperties)
    {
        Require.NotNull(animationFrames, nameof(animationFrames));

        Id = id;
        SourceArea = sourceArea;
        AnimationFrames = [..animationFrames];
    }

    /// <summary>
    /// Gets the local identifier of this tile within its tile set.
    /// </summary>
    public int Id
    { get; }

    /// <summary>
    /// Gets the explicit bounding rectangle of the region of the texture associated with this tile that will be rendered
    /// when drawing this tile, if one exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will only be present if the tile set this tile belongs to was originally designed to be based on a collection
    /// of images.
    /// </para>
    /// <para>
    /// If this is the case, then a packed texture will have been generated during the processing of the related
    /// tile set asset data for the content pipeline. Texture packing is non-deterministic, so we require this extra bit of
    /// metadata to locate the imaging data for the tile.
    /// </para>
    /// </remarks>
    public Rectangle? SourceArea
    { get; }

    /// <summary>
    /// Gets the frames in this tile's animation, if any.
    /// </summary>
    /// <remarks>This will always be empty for tiles that are not animated.</remarks>
    public IReadOnlyList<TileAnimationFrame> AnimationFrames
    { get; }
}
