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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a source of rectangular images, or tiles, for use in a tile map.
/// </summary>
public sealed class TileSet : Extensible
{
    private readonly Dictionary<int, Texture2D> _tileTextures = [];

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
        => Texture != null ? TileCount -1 : _tileTextures.Keys.Max();

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
    /// Adds a tile-specific texture to this tile set.
    /// </summary>
    /// <param name="localId">The tile identifier localized to the tile set.</param>
    /// <param name="texture">The texture for the tile.</param>
    public void AddTile(int localId, Texture2D texture)
    {
        Require.NotNull(texture, nameof(texture));

        if (!_tileTextures.TryAdd(localId, texture))
            throw new ArgumentException(Strings.TileAlreadyHasTexture, nameof(localId));
    }

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
        if (_tileTextures.TryGetValue(localId, out Texture2D? texture))
            return texture.Bounds;

        int x = localId % _columns * (TileSize.Width + Spacing) + Margin;
        int y = localId / _columns * (TileSize.Height + Spacing) + Margin;

        return new Rectangle(x, y, TileSize.Width, TileSize.Height);
    }

    /// <summary>
    /// Gets the texture to source when drawing a tile.
    /// </summary>
    /// <param name="localId">The tile identifier localized to this tile set.</param>
    /// <returns>The texture to source when drawing the tile identified by <c>localId</c>.</returns>
    /// <exception cref="ArgumentException">
    /// The tile set is not based on a single image and <c>localId</c> has no texture associated with it.
    /// </exception>
    public Texture2D GetTileTexture(int localId)
    {
        if (Texture != null)
            return Texture;

        return _tileTextures.GetValueOrDefault(localId)
               ?? throw new ArgumentException(Strings.TileHasNoTexture);
    }
}
