//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Game.Tiles;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile map tile layer asset.
/// </summary>
public sealed class TileLayerAsset : LayerAsset
{
    private const string DATA_ELEMENT = "data";

    /// <summary>
    /// Initializes a new instance of the <see cref="TileLayerAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile layer's configuration.</param>
    public TileLayerAsset(XElement root) 
        : base(root, LayerType.Tile)
    {
        XElement? dataElement = root.Element(DATA_ELEMENT);

        if (dataElement == null)
            throw new InvalidOperationException(Strings.TileLayerNoData);

        Data = new DataAsset(dataElement);

        Width = (int?) root.Attribute(XmlConstants.WidthAttribute) ?? default;
        Height = (int?) root.Attribute(XmlConstants.HeightAttribute) ?? default;
    }

    /// <summary>
    /// Gets the tile data for this layer.
    /// </summary>
    public DataAsset Data
    { get; }

    /// <summary>
    /// Gets the width of this tile layer in tiles.
    /// </summary>
    public int Width
    { get; }

    /// <summary>
    /// Gets the height of this tile layer in tiles.
    /// </summary>
    public int Height
    { get; }

    /// <summary>
    /// Gets the collection of tiles composing this tile layer.
    /// </summary>
    /// <remarks>Meant to be populated during tile map processing.</remarks>
    public ICollection<Tile> Tiles
    { get; } = new List<Tile>();
}
