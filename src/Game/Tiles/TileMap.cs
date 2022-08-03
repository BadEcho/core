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

using BadEcho.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a piece of the game's world, or map of a level, built out of rectangular-shaped tile images, among
/// other kinds of content.
/// </summary>
/// <remarks>
/// Tile maps supported by the Bad Echo Game Framework are based on the TMX map format, used by various popular tile map editors such
/// as Tiled.
/// </remarks>
public sealed class TileMap
{
    private readonly Dictionary<Layer, IEnumerable<IPrimitiveModel>> _layerModelMap = new();
    private readonly Dictionary<TileSet, int> _tileSetFirstIdMap = new();
    private readonly List<TileSet> _tileSets = new();
    private readonly List<Layer> _layers = new();
    private readonly GraphicsDevice _device;
    
    private Matrix _world = Matrix.Identity;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileMap"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the map's models.</param>
    /// <param name="name">The name of the tile map.</param>
    /// <param name="size">The tile map's size, measured in tiles.</param>
    /// <param name="tileSize">The size of the tiles used in the tile map.</param>
    /// <param name="orientation">The orientation of the tile map.</param>
    /// <param name="renderOrder">The order in which tiles on the map are rendered.</param>
    /// <param name="backgroundColor">The background color of the tile map.</param>
    public TileMap(GraphicsDevice device, 
                   string name,
                   Point size,
                   Point tileSize,
                   MapOrientation orientation,
                   TileRenderOrder renderOrder,
                   Color backgroundColor)
    {
        _device = device;

        Name = name;
        Size = size;
        TileSize = tileSize;
        Orientation = orientation;
        RenderOrder = renderOrder;
        BackgroundColor = backgroundColor;
    }

    /// <summary>
    /// Gets the name of this tile map.
    /// </summary>
    /// <remarks>
    /// The TMX map format lacks a name field for a particular tile map; the name used here is essentially based on its
    /// identifying name as a game asset (i.e., the name of the file containing the map data).
    /// </remarks>
    public string Name
    { get; }

    /// <summary>
    /// Gets this tile map's size, measured in tiles.
    /// </summary>
    /// <remarks>
    /// The dimensions of the tile map is determined using tiles (whose sizes are known) as opposed to simply pixels. A 64x64 size,
    /// for example, indicates a map that is a 64 tiles wide by 64 tiles hide, with a total area of 4,096 tiles.
    /// </remarks>
    public Point Size
    { get; }

    /// <summary>
    /// Gets the size of the tiles used in this map.
    /// </summary>
    /// <remarks>
    /// Maps defined using the TMX map format use a uniform size for all the tiles that they comprise. The tile size defined for the
    /// map essentially, then, defines the size of tiles used by all its constituent layers.
    /// </remarks>
    public Point TileSize
    { get; }

    /// <summary>
    /// Gets the orientation of this tile map.
    /// </summary>
    public MapOrientation Orientation
    { get; }

    /// <summary>
    /// Gets the order in which tiles on this map are rendered.
    /// </summary>
    public TileRenderOrder RenderOrder
    { get; }

    /// <summary>
    /// Gets the background color of the tile map.
    /// </summary>
    public Color BackgroundColor
    { get; }

    /// <summary>
    /// Gets the collection of tile sets this map sources its tile images from.
    /// </summary>
    public IReadOnlyCollection<TileSet> TileSets
        => _tileSets;

    /// <summary>
    /// Gets the collection of map layers that compose this tile map.
    /// </summary>
    public IReadOnlyCollection<Layer> Layers
        => _layers;

    /// <summary>
    /// Adds a tile set to this map so its tile data may be sourced when drawing the individual tiles appearing on this map.
    /// </summary>
    /// <param name="tileSet">The tile set to add to this map.</param>
    /// <param name="firstId">
    /// The starting value to use when mapping global identifiers mapped to the tiles in this tile set.
    /// </param>
    public void AddTileSet(TileSet tileSet, int firstId)
    {
        _tileSets.Add(tileSet);
        _tileSetFirstIdMap.Add(tileSet, firstId);
    }
    
    /// <summary>
    /// Adds a layer to this map so that its content is rendered when this map is drawn.
    /// </summary>
    /// <param name="layer">The layer to add to this map.</param>
    public void AddLayer(Layer layer)
    {
        _layers.Add(layer);
    }

    /// <summary>
    /// Draws the tile map to the screen.
    /// </summary>
    /// <param name="view">The view matrix to use.</param>
    public void Draw(Matrix view)
    {
        if (_layerModelMap.Count == 0)
            CreateModels();

        var projection = Matrix.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, -1);

        foreach (var layer in Layers)
        {
            _world.Translation = new Vector3(layer.Offset, 0);

            var effect = new BasicEffect(_device)
                         {
                             World = _world,
                             View = view,
                             Projection = projection
                         };

            foreach (var layerModel in _layerModelMap[layer])
            {
                layerModel.Draw(effect);
            }
        }
    }

    /// <summary>
    /// Generates all the models required to render this tile map when it is being submitted for drawing.
    /// </summary>
    public void CreateModels()
    {
        ClearModels();

        foreach (var layer in _layers)
        {
            switch (layer)
            {
                case ImageLayer imageLayer:
                    CreateLayerModels(imageLayer);
                    break;

                case TileLayer tileLayer:
                    CreateLayerModels(tileLayer);
                    break;
            }
        }
    }

    private void ClearModels()
    {
        IEnumerable<IPrimitiveModel> models = _layerModelMap.SelectMany(lm => lm.Value);

        foreach (var model in models)
        {
            model.Dispose();
        }

        _layerModelMap.Clear();
    }

    private void CreateLayerModels(ImageLayer layer)
    {
        var imageData = new RectangleModelData();

        imageData.AddTexture(layer.Image.Bounds, layer.Image.Bounds, layer.Position);

        _layerModelMap.Add(layer,
                           new StaticModel(_device, layer.Image, imageData).AsEnumerable());
    }

    private void CreateLayerModels(TileLayer layer)
    {
        var layerModels = new List<IPrimitiveModel>();
        
        // Layer tile data must be processed tile set by tile set, as they provide the actual textures that individual tiles are sourced from.
        foreach (TileSet tileSet in TileSets)
        {
            var tileData = new RectangleModelData();
            int firstId = _tileSetFirstIdMap[tileSet];
            Texture2D texture = tileSet.Texture;
            // Extract the tiles in the layer we're adding that belong to the current tile set.
            foreach (Tile tile in layer.GetRange(firstId, tileSet.TileCount))
            {   // Tile identifiers are normalized so that they're unique across the entire map that they're used in.
                // In order to pull a tile's data from its tile set of origin, we'll need to revert its id back into its localized variant.
                int localId = tile.Id - firstId;
                Vector2 position = GetTilePosition(tile);
                Rectangle sourceArea = tileSet.GetTileSourceArea(localId);

                tileData.AddTexture(texture.Bounds, sourceArea, position);

                if (tileData.VertexCount + 4 > ushort.MaxValue)
                {
                    layerModels.Add(new StaticModel(_device, texture, tileData));
                    
                    tileData = new RectangleModelData();
                }
            }

            if (tileData.VertexCount > 0)
                layerModels.Add(new StaticModel(_device, texture, tileData));
        }

        _layerModelMap.Add(layer, layerModels);
    }

    private Vector2 GetTilePosition(Tile tile)
    {
        switch (Orientation)
        {
            case MapOrientation.Orthogonal:
                return new Vector2(tile.ColumnIndex * TileSize.X, tile.RowIndex * TileSize.Y);
            // TODO: Fill out.
            default:
                throw new NotSupportedException();
        }
    }
}
