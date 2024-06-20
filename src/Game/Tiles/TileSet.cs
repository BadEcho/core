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

using BadEcho.Extensions;
using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a source of rectangular images, or tiles, for use in a tile map.
/// </summary>
public sealed class TileSet : Extensible
{
    private readonly Dictionary<int, TileData> _idTileMap = [];

    private readonly int _columns;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileSet"/> class.
    /// </summary>
    /// <param name="texture">The texture containing the individual tiles that compose this tile set.</param>
    /// <param name="tileSize">The size of an individual tile in this tile set.</param>
    /// <param name="tileCount">The number of tiles in this tile set.</param>
    /// <param name="columns">
    /// The number of columns of tiles in this tile set. This will be zero if the tile set was originally based
    /// on a collection images, as editors do not treat the tile set as a grid in that case.
    /// </param>
    /// <param name="customProperties">The tile set's custom properties.</param>
    public TileSet(Texture2D texture,
                   Size tileSize,
                   int tileCount,
                   int columns,
                   CustomProperties customProperties)
        : base(customProperties)
    {
        Require.NotNull(texture, nameof(texture));

        Texture = texture;
        TileSize = tileSize;
        TileCount = tileCount;
        _columns = columns;
    }   

    /// <summary>
    ///  Gets the texture containing the individual tiles that compose this tile set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tile sets can be designed to be either based on a single image, or a collection of images. In the
    /// former case, this texture will reflect that single image.
    /// </para>
    /// <para>
    /// In the latter case, this texture will instead be based on a packed texture that was generated during the
    /// processing of this tile set's asset data for the content pipeline.
    /// </para>
    /// </remarks>
    public Texture2D Texture
    { get; } 

    /// <summary>
    /// Gets size of an individual tile in this tile set.
    /// </summary>
    public Size TileSize
    { get; }

    /// <summary>
    /// Gets the number of tiles in this tile set.
    /// </summary>
    public int TileCount
    { get; }

    /// <summary>
    /// Gets the identifier of the tile representing the upper bound of this tile set.
    /// </summary>
    /// <remarks>
    /// Identifiers for tiles will always be contiguous if the tile set is based on a single image;
    /// however, tile sets that are (at design-time) a collection of images may be non-contiguous if
    /// existing tiles are removed at a later date.
    /// </remarks>
    public int LastId
        => _columns != 0 ? TileCount - 1 : _idTileMap.Keys.Max();

    /// <summary>
    /// Gets the space between the perimeter of the tiles composing this tile set and the edge of the texture.
    /// </summary>
    public int Margin
    { get; init; }

    /// <summary>
    /// Gets the spacing between the individual tiles that compose this tile set.
    /// </summary>
    public int Spacing
    { get; init; }

    /// <summary>
    /// Adds explicitly configured tile data to this tile set.
    /// </summary>
    /// <param name="tile">Data for the tile to add to this tile set.</param>
    public void AddTile(TileData tile)
    {
        Require.NotNull(tile, nameof(tile));
        
        if (!_idTileMap.TryAdd(tile.Id, tile))
            throw new ArgumentException(Strings.TileSetAlreadyHasTile.InvariantFormat(tile.Id));
    }

    /// <summary>
    /// Gets a value indicating if a tile is animated.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>True if the tile identified by <c>localId</c> is animated; otherwise, false.</returns>
    public bool IsTileAnimated(int localId)
        => _idTileMap.TryGetValue(localId, out TileData? tile) && tile.AnimationFrames.Count > 0;

    /// <summary>
    /// Gets the frames for a tile's animation, if any.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>The animation frames for the tile identified by <c>localId</c>, if any.</returns>
    public IReadOnlyList<TileAnimationFrame> GetTileAnimationFrames(int localId)
        => _idTileMap.TryGetValue(localId, out TileData? tile) ? tile.AnimationFrames : [];

    /// <summary>
    /// Gets the bounding rectangle of the region in the texture associated with a tile that will be rendered when
    /// drawing said tile.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>
    /// The bounding rectangle of the region in the texture associated with the tile identified by <c>localId</c>.
    /// </returns>
    public Rectangle GetTileSourceArea(int localId)
    {
        if (_idTileMap.TryGetValue(localId, out TileData? tile))
        {
            if (tile.SourceArea != null)
                return tile.SourceArea.Value;
        }

        int x = localId % _columns * (TileSize.Width + Spacing) + Margin;
        int y = localId / _columns * (TileSize.Height + Spacing) + Margin;

        return new Rectangle(x, y, TileSize.Width, TileSize.Height);
    }

    /// <summary>
    /// Gets the custom properties associated with a tile.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>
    /// A <see cref="CustomProperties"/> instance containing custom properties for the tile identified by <c>localId</c>.
    /// </returns>
    public CustomProperties GetTileCustomProperties(int localId)
        => _idTileMap.TryGetValue(localId, out TileData? tile) ? tile.CustomProperties : new CustomProperties();
}
