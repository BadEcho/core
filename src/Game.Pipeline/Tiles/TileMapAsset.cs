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

using System.Xml.Linq;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Game.Tiles;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile map asset.
/// </summary>
public sealed class TileMapAsset
{
    private const string VERSION_ATTRIBUTE = "version";
    private const string ORIENTATION_ATTRIBUTE = "orientation";
    private const string RENDER_ORDER_ATTRIBUTE = "renderorder";
    private const string BACKGROUND_COLOR_ATTRIBUTE = "backgroundcolor";
    private const string TILE_SET_ELEMENT = "tileset";
    private const string TILE_LAYER_ELEMENT = "layer";
    private const string IMAGE_LAYER_ELEMENT = "imageLayer";

    private readonly List<TileSetAsset> _tileSets = new();
    private readonly List<LayerAsset> _layers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TileMapAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile map's configuration.</param>
    /// <param name="name">The name of the tile map.</param>
    public TileMapAsset(XElement root, string name)
    {
        Require.NotNull(root, nameof(root));

        Name = name;
        Version = (string?) root.Attribute(VERSION_ATTRIBUTE) ?? string.Empty;
        Width = (int?) root.Attribute(XmlConstants.WidthAttribute) ?? default;
        Height = (int?) root.Attribute(XmlConstants.HeightAttribute) ?? default;
        TileWidth = (int?) root.Attribute(XmlConstants.TileWidthAttribute) ?? default;
        TileHeight = (int?) root.Attribute(XmlConstants.TileHeightAttribute) ?? default;
        BackgroundColor = (string?) root.Attribute(BACKGROUND_COLOR_ATTRIBUTE) ?? string.Empty;

        Orientation = ReadOrientation(root);
        RenderOrder = ReadRenderOrder(root);

        foreach (XElement tileSet in root.Elements(TILE_SET_ELEMENT))
        {
            _tileSets.Add(new TileSetAsset(tileSet));
        }

        foreach (XElement tileLayer in root.Elements(TILE_LAYER_ELEMENT))
        {
            _layers.Add(new TileLayerAsset(tileLayer));
        }

        foreach (XElement imageLayer in root.Elements(IMAGE_LAYER_ELEMENT))
        {
            _layers.Add(new ImageLayerAsset(imageLayer));
        }
    }

    /// <summary>
    /// Gets the name of this tile map.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets the TMX format version for this tile map.
    /// </summary>
    public string Version
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
    /// Gets the background color, expressed as a hex color code, of the map.
    /// </summary>
    public string BackgroundColor
    { get; }

    /// <summary>
    /// Gets the width of this tile map in tiles.
    /// </summary>
    public int Width 
    { get; }

    /// <summary>
    /// Gets the height of this tile map in tiles.
    /// </summary>
    public int Height
    { get; }

    /// <summary>
    /// Gets the width of an individual tile in this tile map.
    /// </summary>
    public int TileWidth
    { get; }

    /// <summary>
    /// Gets the height of an individual tile in this tile map.
    /// </summary>
    public int TileHeight
    { get; }

    /// <summary>
    /// Gets the collection of tile sets this map sources its tile images from.
    /// </summary>
    public IEnumerable<TileSetAsset> TileSets
        => _tileSets;

    /// <summary>
    /// Gets the collection of map layers that compose this tile map.
    /// </summary>
    public IEnumerable<LayerAsset> Layers
        => _layers;

    private TileRenderOrder ReadRenderOrder(XElement root)
    {
        string? renderOrderValue = (string?)root.Attribute(RENDER_ORDER_ATTRIBUTE);

        if (string.IsNullOrEmpty(renderOrderValue))
            throw new InvalidOperationException(Strings.TileMapMissingRenderOrder);

        if (!Enum.TryParse(renderOrderValue, out TileRenderOrder renderOrder))
        {
            throw new NotSupportedException(
                Strings.TileMapUnsupportedRenderOrder.InvariantFormat(Name, renderOrderValue));
        }

        return renderOrder;
    }

    private MapOrientation ReadOrientation(XElement root)
    {
        string? orientationValue = (string?)root.Attribute(ORIENTATION_ATTRIBUTE);

        if (string.IsNullOrEmpty(orientationValue))
            throw new InvalidOperationException(Strings.TileMapMissingOrientation);

        if (!Enum.TryParse(orientationValue, out MapOrientation orientation))
        {
            throw new NotSupportedException(
                Strings.TileMapUnsupportedOrientation.InvariantFormat(Name, orientationValue));
        }

        return orientation;
    }
}
