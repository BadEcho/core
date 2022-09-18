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
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile map asset.
/// </summary>
public sealed class TileMapAsset : ExtensibleAsset
{
    private const string VERSION_ATTRIBUTE = "version";
    private const string ORIENTATION_ATTRIBUTE = "orientation";
    private const string RENDER_ORDER_ATTRIBUTE = "renderorder";
    private const string BACKGROUND_COLOR_ATTRIBUTE = "backgroundcolor";
    private const string TILE_SET_ELEMENT = "tileset";
    private const string TILE_LAYER_ELEMENT = "layer";
    private const string IMAGE_LAYER_ELEMENT = "imagelayer";

    private readonly List<TileSetAsset> _tileSets = new();
    private readonly List<LayerAsset> _layers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TileMapAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile map's configuration.</param>
    public TileMapAsset(XElement root)
        : base(root)
    {
        Require.NotNull(root, nameof(root));

        Version = (string?) root.Attribute(VERSION_ATTRIBUTE) ?? string.Empty;
        Width = (int?) root.Attribute(XmlConstants.WidthAttribute) ?? default;
        Height = (int?) root.Attribute(XmlConstants.HeightAttribute) ?? default;
        TileWidth = (int?) root.Attribute(XmlConstants.TileWidthAttribute) ?? default;
        TileHeight = (int?) root.Attribute(XmlConstants.TileHeightAttribute) ?? default;
        string backgroundColorHex = (string?) root.Attribute(BACKGROUND_COLOR_ATTRIBUTE) ?? string.Empty;

        BackgroundColor = string.IsNullOrEmpty(backgroundColorHex)
            ? Color.Transparent
            : backgroundColorHex.ToColor();

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
    /// Gets the background color of the map.
    /// </summary>
    public Color BackgroundColor
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
    public IReadOnlyCollection<TileSetAsset> TileSets
        => _tileSets;

    /// <summary>
    /// Gets the collection of map layers that compose this tile map.
    /// </summary>
    public IReadOnlyCollection<LayerAsset> Layers
        => _layers;

    private static TileRenderOrder ReadRenderOrder(XElement root)
    {
        string? renderOrderValue = (string?) root.Attribute(RENDER_ORDER_ATTRIBUTE);

        if (string.IsNullOrEmpty(renderOrderValue))
            throw new InvalidOperationException(Strings.TileMapMissingRenderOrder);
        
        renderOrderValue = renderOrderValue.Replace("-", string.Empty, StringComparison.Ordinal);

        if (!Enum.TryParse(renderOrderValue, true, out TileRenderOrder renderOrder))
        {
            throw new NotSupportedException(
                Strings.TileMapUnsupportedRenderOrder.InvariantFormat(renderOrderValue));
        }

        return renderOrder;
    }

    private static MapOrientation ReadOrientation(XElement root)
    {
        string? orientationValue = (string?)root.Attribute(ORIENTATION_ATTRIBUTE);

        if (string.IsNullOrEmpty(orientationValue))
            throw new InvalidOperationException(Strings.TileMapMissingOrientation);

        if (!Enum.TryParse(orientationValue, true, out MapOrientation orientation))
        {
            throw new NotSupportedException(
                Strings.TileMapUnsupportedOrientation.InvariantFormat(orientationValue));
        }

        return orientation;
    }
}
