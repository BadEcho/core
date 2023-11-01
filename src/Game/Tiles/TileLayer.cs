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

using BadEcho.Extensions;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides an area in a tile map filled with tile data.
/// </summary>
public sealed class TileLayer : Layer
{
    private readonly Dictionary<int, List<int>> _tileIdIndicesMap = new();
    private readonly Tile[] _tiles;
    private readonly Size _tileSize;
    private readonly Size _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="size">
    /// The tile layer's size, measured in tiles (i.e., a 4x4 size indicates four tiles wide by four tiles high, with a total
    /// area of 16 tiles).
    /// </param>
    /// <param name="tileSize">The size of the tiles used in this layer.</param>
    /// <param name="customProperties">The tile layer's custom properties.</param>
    public TileLayer(string name,
                     Size size,
                     Size tileSize,
                     CustomProperties customProperties)
        : base(name, customProperties)
    {
        _size = size;
        _tileSize = tileSize;
        _tiles = new Tile[size.Width * size.Height];
    }

    /// <summary>
    /// Gets the tiles belonging to this tile layer.
    /// </summary>
    public IReadOnlyCollection<Tile> Tiles
        => _tiles;

    /// <summary>
    /// Gets a range of tiles found within this tile layer.
    /// </summary>
    /// <param name="firstId">The global identifier of the tile at which the range starts.</param>
    /// <param name="maxCount">The maximum number of elements in the range.</param>
    /// <returns>A range of up to <c>maxCount</c> tiles starting at the tile identified by <c>firstId</c>.</returns>
    public IEnumerable<Tile> GetRange(int firstId, int maxCount)
    {
        int lastId = maxCount + firstId - 1;

        for (int i = firstId; i <= lastId; i++)
        {
            if (!_tileIdIndicesMap.TryGetValue(i, out List<int>? tileIndices))
                continue;

            foreach (int tileIndex in tileIndices)
            {
                yield return _tiles[tileIndex];
            }
        }
    }

    /// <summary>
    /// Gets the tile, if any, found at the specified position in the layer.
    /// </summary>
    /// <param name="position">The drawing location of the tile to return.</param>
    /// <returns>The tile being drawn at <c>position</c>.</returns>
    public Tile GetTile(Vector2 position)
    {
        int column = (int) position.X / _tileSize.Width;
        int row = (int) position.Y / _tileSize.Height;
        int index = CalculateTileIndex(column, row);

        return _tiles[index];
    }

    /// <summary>
    /// Loads the identified tile into the tile layer at the specified location.
    /// </summary>
    /// <param name="idWithFlags">The global identifier and flip flags of the tile to load into the layer.</param>w
    /// <param name="column">The index of the column within the tile layer to load the tile into.</param>
    /// <param name="row">The index of the row within the tile layer to load the tile into.</param>
    public void LoadTile(uint idWithFlags, int column, int row)
    {
        int index = CalculateTileIndex(column, row);
        
        var tile = new Tile(idWithFlags, column, row);
        var tileIndices = _tileIdIndicesMap.GetValueOrAdd(tile.Id, _ => new List<int>());

        _tiles[index] = tile;
        tileIndices.Add(index);
    }

    /// <summary>
    /// Converts this tile layer into a sequence of space-occupying entities for all tiles.
    /// </summary>
    /// <returns>A sequence of <see cref="ISpatialEntity"/> instances for every tile in this layer.</returns>
    internal IEnumerable<ISpatialEntity> ToSpatialLayer()
    {
        var validTiles = Tiles.Select((t, i) => new { Tile = t, Index = i })
                              .Where(ti => ti.Tile != default);

        foreach (var validTile in validTiles)
        {
            int row = validTile.Index / _size.Width;
            int column = row == 0 ? validTile.Index : validTile.Index % (row * _size.Width);

            yield return new TileSpatialEntity(column, row, _tileSize);
        }
    }

    private int CalculateTileIndex(int column, int row)
        => column + row * _size.Width;

    /// <summary>
    /// Provides spatial boundaries for a collidable tile.
    /// </summary>
    private sealed class TileSpatialEntity : ISpatialEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileSpatialEntity"/> class.
        /// </summary>
        /// <param name="column">The index of the column within the tile layer that the tile occupies.</param>
        /// <param name="row">The index of the row within the tile layer that the tile occupies.</param>
        /// <param name="tileSize">The size of the tile.</param>
        public TileSpatialEntity(int column, int row, Size tileSize)
        {
            float x = column * tileSize.Width;
            float y = row * tileSize.Height;

            Bounds = new RectangleF(x, y, tileSize.Width, tileSize.Height);
        }

        /// <inheritdoc />
        public IShape Bounds 
        { get; }

        /// <inheritdoc />
        public void ResolveCollision(IShape shape)
        { }
    }
}