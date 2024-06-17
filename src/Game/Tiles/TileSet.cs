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
    /// <param name="texture">The texture containing the individual tiles that compose this tile set, if one exists.</param>
    /// <param name="tileSize">The size of an individual tile in this tile set.</param>
    /// <param name="tileCount">The number of tiles in this tile set.</param>
    /// <param name="columns">The number of columns of tiles in this tile set.</param>
    /// <param name="customProperties">The tile set's custom properties.</param>
    public TileSet(Texture2D? texture,
                   Size tileSize,
                   int tileCount,
                   int columns,
                   CustomProperties customProperties)
        : base(customProperties)
    {
        Texture = texture;
        TileSize = tileSize;
        TileCount = tileCount;
        _columns = columns;
    }   

    /// <summary>
    ///  Gets the texture containing the individual tiles that compose this tile set, if one exists.
    /// </summary>
    /// <remarks>
    /// This will be present if the tile set is based on a single image. If a tile set is based on a collection
    /// of images, then the tile set's constituent tile assets will contain the texture data.
    /// </remarks>
    public Texture2D? Texture
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
    /// however, tile sets that are a collection of images may be non-contiguous if existing tiles are
    /// removed at a later date.
    /// </remarks>
    public int LastId
        => Texture != null ? TileCount -1 : _idTileMap.Keys.Max();

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

    public bool IsTileAnimated(int localId)
        => _idTileMap.TryGetValue(localId, out TileData? tile) && tile.AnimationFrames.Count > 0;

    public IEnumerable<TileAnimationFrame> GetTileAnimationFrames(int localId)
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
            if (tile.Texture != null)
                return tile.Texture.Bounds;
        }

        int x = localId % _columns * (TileSize.Width + Spacing) + Margin;
        int y = localId / _columns * (TileSize.Height + Spacing) + Margin;

        return new Rectangle(x, y, TileSize.Width, TileSize.Height);
    }

    /// <summary>
    /// Gets the texture to source when drawing a tile.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>The texture to source when drawing the tile identified by <c>localId</c>.</returns>
    public Texture2D GetTileTexture(int localId)
    {
        if (Texture != null)
            return Texture;

        if (!_idTileMap.TryGetValue(localId, out TileData? tile))
            throw new ArgumentException(Strings.TileSetMissingTile, nameof(localId));
        
        return tile.Texture
               ?? throw new InvalidOperationException(Strings.TileHasNoTexture);
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
